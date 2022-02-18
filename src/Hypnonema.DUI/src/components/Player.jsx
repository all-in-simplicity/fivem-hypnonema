import React from 'react';
import ReactPlayer from 'react-player';
import {useState, useRef} from 'react';
import useNuiMessage from '../hooks/useNuiMessage';

const Player = () => {
    const player = useRef(null);

    const [isVisible, setIsVisible] = useState(true);

    const [url, setUrl] = useState('');

    const [resourceName, setResourceName]  = useState('hypnonema');

    const [posterUrl, setPosterUrl] = useState('https://i.imgur.com/dPaIjEW.jpg');

    const [screenName, setScreenName] = useState('');

    const [playing, setPlaying] = useState(false);

    const [volume, setVolume] = useState(0.0);

    const [muted, setMuted] = useState(false);

    const [loop, setLoop] = useState(false);

    const onPlay = ({payload}) => {
        if(!ReactPlayer.canPlay(payload)) {
            console.log(`playback aborted: cant play ${payload}`);
            return;
        }

        setUrl(payload);
        setPlaying(true);
    }

    const onStop = () => {
        setPlaying(false);
        setUrl('');
    }

    const onInit = ({payload}) => {
        const { resourceName, screenName, posterUrl} = payload;

        setScreenName(screenName);
        setResourceName(resourceName);
        setPosterUrl(posterUrl);
    }

    const onPause = () => {
        setPlaying(false);
    }

    const onResume = () => {
        setPlaying(true);
    }

    const onSeek = ({payload}) => {
        player.current.seekTo(payload, 'seconds');
    }

    const onLoop = () => {
        setLoop(!loop);
    }

    const onVolume = ({payload}) => {
        const vol = parseFloat(payload);

        setVolume(vol);
    }

    const onSynchronizeState = ({payload}) => {
        const {url, paused, currentTime, looped} = payload;

        setUrl(url);

        if (!paused) {
            setPlaying(true);
        }

        player.current.seekTo(currentTime, 'seconds');
        setLoop(looped);
    }

    const onReady = () => {
        if(player.current.player.player.constructor.name === 'Twitch') {
            const internalPlayer = player.current.getInternalPlayer();

            const button = internalPlayer._iframe.contentWindow.document.querySelector('button[data-a-target="player-overlay-mature-accept"]');
            if (button) {
                button.click();
            }
        }
    }

    const onEnded = () => {
        if (!loop) {
            setTimeout(() => {
                sendDuiResponse('playbackEnded', { screenName}).then(() => {});
            }, 2500);
        }
    }

    const onDuration = (duration) => {
        sendDuiResponse('updateStateDuration', {screenName, duration}).then(() => {});
    }

    const onShowPlayer = ({payload}) => {
        setIsVisible(payload);
    }

    useNuiMessage('showPlayer', onShowPlayer);
    useNuiMessage('synchronizeState', onSynchronizeState);
    useNuiMessage('volume', onVolume);
    useNuiMessage('loop', onLoop);
    useNuiMessage('seek', onSeek);
    useNuiMessage('resume', onResume);
    useNuiMessage('play', onPlay);
    useNuiMessage('stop', onStop);
    useNuiMessage('init', onInit);
    useNuiMessage('pause', onPause);

    const sendDuiResponse = (nuiCallback, body) => {
        const url = new URL(nuiCallback, `${window.location.protocol}${resourceName}`).toString();

        return fetch(url, {
            headers: {'content-type': 'application/json; charset=UTF-8'},
            method: 'POST',
            body: JSON.stringify(body)
        }).catch(error => console.log(error));
    };

    if (!isVisible) {
        return null;
    }

    return (
        <div style={{width: '100%', height: '100%'}}>
            {!playing &&
            <div id='posterImg'>
                <img src={posterUrl} alt=''/>
            </div>
            }
            <div className='player-wrapper'>
                <ReactPlayer
                    ref={player}
                    className='react-player'
                    url={url}
                    pip={false}
                    playing={playing}
                    controls={false}
                    loop={loop}
                    playbackRate={1.0}
                    volume={volume}
                    muted={muted}
                    onDuration={onDuration}
                    onReady={onReady}
                    onEnded={onEnded}
                    onError={e => console.log('onError', JSON.stringify(e))}
                    width='100%'
                    height='100%'
                />
            </div>
        </div>
    )
}
export default Player;
