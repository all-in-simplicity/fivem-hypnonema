import {useCallback, useEffect, useState} from 'react';

function useNuiMessage(nuiMessage, eventHandler) {
    const [history, setHistory] = useState([]);

    const onWatchEventHandler = useCallback(({data}) => {
        const {type, payload} = data;

        if (type === nuiMessage) {
            setHistory(old => [...old, payload]);
            eventHandler(payload);
        }

    }, [nuiMessage, eventHandler]);

    useEffect(() => {
        window.addEventListener('message', onWatchEventHandler);
        return () => window.removeEventListener('message', onWatchEventHandler);
    }, [onWatchEventHandler]);

    return {history};
}

export default useNuiMessage;
