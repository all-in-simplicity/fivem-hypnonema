import React, {Component} from 'react';
import ReactPlayer from "react-player";
import Audio3D from "../utils/audio-3d";

class App extends Component {
    constructor(props) {
        super(props);
        this.handleMessage = this.handleMessage.bind(this);
        this.handleStateTick = this.handleStateTick.bind(this);
        this.audio3D = new Audio3D();
    }

    state = {
        url: '',
        previousHost: '',
        poster: 'https://i.imgur.com/dPaIjEW.jpg',
        pausePoster: 'https://i.imgur.com/aoz9sJn.jpg',
        showPoster: true,
        pip: false,
        playing: true,
        controls: false,
        volume: 1.0,
        muted: false,
        played: 0,
        loaded: 0,
        duration: 0,
        playbackRate: 1.0,
        loop: false,
        screenName: '',
        tickInterval: 10000,
        panningOpts: {
            panningModel: 'HRTF',
            distanceModel: 'exponential',
            refDistance: 15,
            maxDistance: 2000,
            rolloffFactor: 0.6,
            coneInnerAngle: 360,
            coneOuterAngle: 360,
            coneOuterGain: 0.5,
        }
    };

    load = url => {
        this.setState({
            url,
            played: 0,
            loaded: 0,
            pip: false,
        })
    };

    // although not very nice, this method removes branding and stuff..
    // TODO: Add support for more services than just YouTube?
    adsSuck = () => {
        const css = `
        div.ytp-chrome-top.ytp-show-watch-later-title.ytp-share-button-visible.ytp-show-share-title.ytp-show-cards-title {
            display: none;
        }
        a .ytp-watermark.yt-uix-sessionlink {
            display: none;
        }
        `;
        const iframeRef = document.getElementsByTagName('iframe')[0].contentDocument;
        const head = iframeRef.getElementsByTagName('head')[0];
        const style = iframeRef.createElement('style');
        head.appendChild(style);

        style.setAttribute('type', 'text/css');
        style.appendChild(iframeRef.createTextNode(css));
    };

    loadAndPlay = url => {
        this.load(url);
        this.setState({playing: true, muted: false});
    };

    handleStop = () => {
        this.setState({url: null, playing: false})
    };

    handleToggleLoop = () => {
        this.setState({loop: !this.state.loop})
    };

    handlePlay = () => {
        this.setState({playing: true})
    };

    handlePause = () => {
        this.setState({playing: false})
    };

    handleEnded = () => {
        this.setState({playing: this.state.loop})
    };

    handleDuration = (duration) => {
        this.setState({duration})
    };

    handleVolume = (volume) => {
        this.setState({volume: parseFloat(volume)})
    };

    handleSeek = (time) => {
        this.player.seekTo(time, 'seconds');
    };

    handleInit = (screenName, posterUrl) => {
        this.setState({screenName: screenName, poster: posterUrl})
    };

    sendDuiResponse = (url, body) => {
        fetch(url, {
            headers: {'content-type': 'application/json; charset=UTF-8'},
            method: 'POST',
            body: JSON.stringify(body)
        }).catch(error => console.log(error));
    };

    getPlayerState = () => {
        return {
            paused: !this.state.playing,
            currentTime: this.player.getCurrentTime(),
            duration: this.state.duration,
            currentSource: this.state.url,
            ended: this.player.getCurrentTime() === this.player.getDuration(),
            screenName: this.state.screenName
        }
    };

    handleStart = () => {
        const newURL = new URL(this.state.url);
        const source = this.getSource();

        if (this.state.previousHost !== newURL.host) {
            this.adsSuck();
            this.audio3D.init(this.state.panningOpts, source);
            this.setState({previousHost: newURL.host});
        }
    };

    getSource = () => {
        const source = document.getElementsByTagName('iframe')[0];
        if (source) {
            return source.contentDocument.getElementsByTagName('video')[0];
        }

        return document.getElementsByTagName('video')[0];
    };

    handleStateTick = () => {
        const url = 'https://hypnonema/Hypnonema.StateTick';
        const body = this.getPlayerState();
        this.sendDuiResponse(url, body);
    };

    handleGetState = () => {
        const url = 'https://hypnonema/Hypnonema.GetStateResponse';
        const body = this.getPlayerState();
        this.sendDuiResponse(url, body);
    };

    handleTick = (listenerObj, pannerObj) => {
        this.audio3D.onTick(listenerObj, pannerObj);
    };

    handleMessage = (ev) => {
        switch (ev.data.type) {
            case 'init':
                // TODO: Implement panningVars send from Client
                this.handleInit(ev.data.screenName, ev.data.posterUrl);
                break;
            case 'play':
                this.loadAndPlay(ev.data.src.url);
                break;
            case 'tick':
                this.handleTick(ev.data.listenerObj, ev.data.pannerObj);
                break;
            case 'update':
                if (!ev.data.paused) {
                    this.loadAndPlay(ev.data.src)
                } else {
                    this.load(ev.data.src);
                }

                this.handleSeek(ev.data.currentTime);
                break;
            case 'seek':
                this.handleSeek(ev.data.time);
                break;
            case 'getState':
                this.handleGetState();
                break;
            case 'pause':
                this.handlePause();
                break;
            case 'stop':
                this.handleStop();
                break;
            case 'resume':
                this.handlePlay();
                break;
            case 'volume':
                this.handleVolume(ev.data.volume);
                break;
            case 'toggleReplay':
                this.handleToggleLoop();
                break;
            default:
                break;
        }
    };

    ref = player => {
        this.player = player;
    };

    componentDidMount() {
        window.addEventListener('message', this.handleMessage, false);
    }

    componentWillUnmount() {
        window.removeEventListener('message', this.handleMessage, false);
    }

    render() {
        const {url, showPoster, poster, playing, controls, volume, muted, loop, playbackRate, pip} = this.state;
        return (
            <div style={{width: '100%', height: '100%'}}>
                {!playing && showPoster &&
                <div id='posterImg'>
                    <img src={poster} alt=''/>
                </div>
                }
                <div className='player-wrapper'>
                    <ReactPlayer
                        ref={this.ref}
                        className='react-player'
                        url={url}
                        pip={pip}
                        playing={playing}
                        controls={controls}
                        loop={loop}
                        playbackRate={playbackRate}
                        volume={volume}
                        muted={muted}
                        onPlay={this.handlePlay}
                        onPause={this.handlePause}
                        onEnded={this.handleEnded}
                        onStart={this.handleStart}
                        onError={e => console.log('onError', e)}
                        onDuration={this.handleDuration}
                        width='100%'
                        height='100%'
                        config={{
                            youtube: {
                                playerVars: {
                                    autoplay: true,
                                }
                            },
                        }}
                    />
                </div>
            </div>
        )
    }
}

export default App;
