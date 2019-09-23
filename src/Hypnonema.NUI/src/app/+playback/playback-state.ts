import { Action, Selector, State, StateContext } from '@ngxs/store';

export interface PlaybackStateModel {
  currentlyPlaying: boolean;
  video: {
    videoType: string;
    videoUrl: string;
  };
}

export class PlaybackStartedAction {
  static readonly type = '[Playback] Playback Started';
  constructor(public videoType: string, public url: string) { }
}

export class PlaybackFinishedAction {
  static readonly type = '[Playback] Playback finished';
}

@State<PlaybackStateModel>({
  name: 'playback',
  defaults: {
    currentlyPlaying: false,
    video: {
      videoType: '',
      videoUrl: ''
    }
  }
})
export class PlaybackState {
  @Selector()
  static isCurrentlyPlaying(state: PlaybackStateModel) {
    return state.currentlyPlaying;
  }

  @Action(PlaybackStartedAction)
  playbackStart(ctx: StateContext<PlaybackStateModel>, { videoType, url}: PlaybackStartedAction) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      currentlyPlaying: true,
      video: {
        videoType,
        videoUrl: url
      }
    });
  }

  @Action(PlaybackFinishedAction)
  playbackEnd(ctx: StateContext<PlaybackStateModel>) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      currentlyPlaying: false,
      video: {
        videoType: '',
        videoUrl: ''
      }
    });
  }
}
