import React, {useRef, useState} from 'react';
import ReactPlayer from 'react-player';
import useNuiMessage from '../hooks/useNuiMessage';

const Player = () => {
    const player = useRef(null);

    const [isVisible, setIsVisible] = useState(true);

    const [url, setUrl] = useState('');

    const [resourceName, setResourceName] = useState('hypnonema');

    const [posterUrl, setPosterUrl] = useState('https://i.imgur.com/dPaIjEW.jpg');

    const [screenName, setScreenName] = useState('');

    const [playing, setPlaying] = useState(false);

    const [volume, setVolume] = useState(0.0);

    const [muted, setMuted] = useState(false);

    const [repeat, setRepeat] = useState(false);

    const onPlay = ({payload}) => {
        setUrl(payload);
        setPlaying(true);
    }

    const onStop = () => {
        setPlaying(false);
        setUrl('');
    }

    const onInit = ({payload}) => {
        const {resourceName, screenName, posterUrl} = payload;

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

    const onRepeat = ({payload}) => {
        setRepeat(payload);
    }

    const onVolume = ({payload}) => {
        const vol = parseFloat(payload);

        setVolume(vol);
    }

    const onSynchronizeState = ({payload}) => {
        const {url, paused, currentTime, repeat} = payload;

        setUrl(url);

        if (!paused) {
            setPlaying(true);
        }

        player.current.seekTo(currentTime, 'seconds');
        setRepeat(repeat);
    }

    const onReady = () => {
        if (player.current.player.player.constructor.name === 'Twitch') {
            const internalPlayer = player.current.getInternalPlayer();

            const button = internalPlayer._iframe.contentWindow.document.querySelector('button[data-a-target="player-overlay-mature-accept"]');
            if (button) {
                button.click();
            }
        }
    }

    const onStart = () => {
        sendDuiResponse('playbackStart', {screenName, date: new Date().toISOString()}).then(() => {});
    }

    const onEnded = () => {
        setTimeout(() => {
            sendDuiResponse('playbackEnded', {screenName}).then(() => {
            });
        }, 2500);
    }

    const onDuration = (duration) => {
        sendDuiResponse('updateStateDuration', {screenName, duration}).then(() => {
        });
    }

    useNuiMessage('synchronizeState', onSynchronizeState);
    useNuiMessage('volume', onVolume);
    useNuiMessage('repeat', onRepeat);
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
                    loop={repeat}
                    playbackRate={1.0}
                    volume={volume}
                    muted={muted}
                    onDuration={onDuration}
                    onReady={onReady}
                    onEnded={onEnded}
                    onStart={onStart}
                    onError={e => console.log('onError', JSON.stringify(e))}
                    width='100%'
                    height='100%'
                />
            </div>
        </div>
    )
}
export default Player;
