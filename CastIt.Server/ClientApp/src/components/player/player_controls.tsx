import { Grid, IconButton } from '@material-ui/core';
import { useEffect, useState } from 'react';
import { SkipPrevious, SkipNext, FastForward, FastRewind, PlayArrow, Stop, Pause } from '@material-ui/icons';
import { onPlayerStatusChanged } from '../../services/castithub.service';
import { useCastItHub } from '../../context/castit_hub.context';

interface State {
    isPlayingOrPaused: boolean;
    isPlaying: boolean;
    isPaused: boolean;
}

const initialState: State = {
    isPlayingOrPaused: false,
    isPlaying: false,
    isPaused: false,
};

function PlayerControls() {
    const [state, setState] = useState(initialState);
    const castItHub = useCastItHub();

    useEffect(() => {
        const onPlayerStatusChangedSubscription = onPlayerStatusChanged.subscribe((status) => {
            if (!status) {
                return;
            }
            setState({
                isPaused: status.player.isPaused,
                isPlaying: status.player.isPlaying,
                isPlayingOrPaused: status.player.isPlayingOrPaused,
            });
        });
        return () => {
            onPlayerStatusChangedSubscription.unsubscribe();
        };
    }, []);

    const handleSkipOrPrevious = async (next: boolean, previous: boolean): Promise<void> => {
        await castItHub.connection.goTo(next, previous);
    };

    const handleGoToSeconds = async (negative: boolean): Promise<void> => {
        await castItHub.connection.skipSeconds(negative ? -30 : 30);
    };

    const handleStopPlayback = async (): Promise<void> => {
        await castItHub.connection.stopPlayBack();
    };

    const handleTogglePlayBack = async (): Promise<void> => {
        await castItHub.connection.togglePlayBack();
    };

    return (
        <Grid container alignItems="stretch" justifyContent="center">
            <IconButton disabled={!state.isPlayingOrPaused} onClick={() => handleSkipOrPrevious(false, true)}>
                <SkipPrevious fontSize="large" />
            </IconButton>
            <IconButton disabled={!state.isPlayingOrPaused} onClick={() => handleGoToSeconds(true)}>
                <FastRewind fontSize="large" />
            </IconButton>
            <IconButton disabled={!state.isPlayingOrPaused} onClick={handleTogglePlayBack}>
                {state.isPaused ? <PlayArrow fontSize="large" /> : <Pause fontSize="large" />}
            </IconButton>
            <IconButton disabled={!state.isPlayingOrPaused} onClick={handleStopPlayback}>
                <Stop fontSize="large" />
            </IconButton>
            <IconButton disabled={!state.isPlayingOrPaused} onClick={() => handleGoToSeconds(false)}>
                <FastForward fontSize="large" />
            </IconButton>
            <IconButton disabled={!state.isPlayingOrPaused} onClick={() => handleSkipOrPrevious(true, false)}>
                <SkipNext fontSize="large" />
            </IconButton>
        </Grid>
    );
}

export default PlayerControls;
