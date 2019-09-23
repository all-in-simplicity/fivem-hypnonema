import { Video } from '@hypnonema/models/video';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { append, patch } from '@ngxs/store/operators';

export interface History {
  video: Video;
  time: Date;
}

export interface HistoryStateModel {
  history: History[];
}

export class AddHistoryEntry {
  static readonly type = '[History] Add History Entry';
  constructor(public videoType: string, public url: string) { }
}

@State<HistoryStateModel>({
  name: 'history',
  defaults: {
    history: []
  }
})
export class HistoryState {
  @Selector()
  static getHistory(state: HistoryStateModel) {
    return state.history;
  }
  @Action(AddHistoryEntry)
  add(ctx: StateContext<HistoryStateModel>, { videoType, url}: AddHistoryEntry) {
    ctx.setState(patch({
      history: append([{video: {type: videoType, url}, time: new Date()}])
    }));
  }
}




