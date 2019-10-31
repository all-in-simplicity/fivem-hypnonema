import { Action, Selector, State, StateContext } from '@ngxs/store';
import { append, patch } from '@ngxs/store/operators';
import { ScreenModel, ScreenStatus } from './screen-model';

export interface AppStateModel {
  isAceAllowed: boolean;
  screens: ScreenModel[];
  selectedScreen: ScreenModel;
  controlledScreen: string;
  screenStatus: ScreenStatus[];
  isLoading: boolean;
}

export class SetIsAceAllowed {
  static readonly type = '[App-State] Setting IsAceAllowed';
  constructor(public isAceAllowed: boolean) { }
}

export class SetScreens {
  static readonly type = '[App-State] Set ScreenModel';
  constructor(public screens: ScreenModel[]) { }
}

export class SetSelectedScreen {
  static readonly type = '[App-State] Set Selected ScreenModel';
  constructor(public payload: ScreenModel) { }
}

export class UpdateScreen {
  static readonly type = '[App-State] Update ScreenModel';
  constructor(public payload: ScreenModel) { }
}

export class DeleteScreen {
  static readonly type = '[App-State] Delete ScreenModel';
  constructor(public screenName: string) { }
}

export class CreateScreen {
  static readonly type = '[App-State] Create ScreenModel';
  constructor(public payload: ScreenModel) { }
}

export class UpdateStatuses {
  static readonly type = '[App-State] Update Statuses';
  constructor(public payload: ScreenStatus[]) { }
}

export class SetIsLoading {
  static readonly type = '[App-State] Set Is Loading';
  constructor(public payload: boolean) { }
}

export class SetControlledScreen {
  static readonly type = '[App-State] Set Controlled Screen';
  constructor(public payload: string) { }
}

export class ClearControlledScreen {
  static readonly type = '[App-State] Clear Selected Screen';
  constructor() { }
}

@State<AppStateModel>({
  name: 'app',
  defaults: {
    isAceAllowed: false,
    screens: [],
    selectedScreen: null,
    screenStatus: null,
    isLoading: false,
    controlledScreen: '',
  }
})
export class AppState {
  @Selector()
  static isAceAllowed(state: AppStateModel) {
    return state.isAceAllowed;
  }

  @Selector()
  static getScreens(state: AppStateModel) {
    return state.screens;
  }

  @Selector()
  static getStatuses(state: AppStateModel) {
    return state.screenStatus;
  }

  @Selector()
  static getControlledScreen(state: AppStateModel) {
    return state.screenStatus.find(s => s.screenName === state.controlledScreen);
  }

  @Selector()
  static getSelectedScreen(state: AppStateModel) {
    return state.selectedScreen;
  }

  @Selector()
  static isLoading(state: AppStateModel) {
    return state.isLoading;
  }

  @Action(SetIsLoading)
  setIsLoading(ctx: StateContext<AppStateModel>, { payload}: SetIsLoading) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      isLoading: payload
    });
  }

  @Action(DeleteScreen)
  deleteScreen({getState, setState}: StateContext<AppStateModel>, {screenName}: DeleteScreen) {
    const state = getState();
    const filteredArray = state.screens.filter(item => item.name !== screenName);
    setState({
      ...state,
      screens: filteredArray
    });
  }

  @Action(UpdateStatuses)
  updateStatuses(ctx: StateContext<AppStateModel>, { payload}: UpdateStatuses) {
    const state = ctx.getState();
    /* check if controlled screen is set and still exists
    if (state.controlledScreen !== '' && payload.find(s => s.screenName === state.controlledScreen) !== undefined) {

    }*/
    ctx.setState({
      ...state,
      screenStatus: payload,
    });
  }

  @Action(ClearControlledScreen)
  clearControlledScreen(ctx: StateContext<AppStateModel>, { }: ClearControlledScreen) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      controlledScreen: ''
    });
  }

  @Action(UpdateScreen)
  updateScreen({getState, setState}: StateContext<AppStateModel>, { payload}: UpdateScreen ) {
    const state = getState();
    const screens = [...state.screens];
    const screenIndex = screens.findIndex(item => item.id === payload.id);
    screens[screenIndex] = payload;
    setState({
      ...state,
      screens
    });
  }

  @Action(CreateScreen)
  createScreen({getState, setState}: StateContext<AppStateModel>, { payload }: CreateScreen) {
    const state = getState();
    const screens = [...state.screens];
    screens.push(payload);
    setState({
      ...state,
      screens
    });
  }

  @Action(SetControlledScreen)
  setControlledScreen(ctx: StateContext<AppStateModel>, { payload}: SetControlledScreen) {
    const state = ctx.getState();
    const screenStates = [...state.screenStatus];
    const screenIndex = screenStates.findIndex(item => item.screenName === payload);
    ctx.setState({
      ...state,
      controlledScreen: state.screenStatus[screenIndex].screenName
    });
  }

  @Action(SetSelectedScreen)
  setSelectedScreen({getState, setState}: StateContext<AppStateModel>, {payload}: SetSelectedScreen) {
    const state = getState();
    setState({
      ...state,
      selectedScreen: payload,
    });
  }

  @Action(SetScreens)
  addScreen(ctx: StateContext<AppStateModel>, { screens }: SetScreens) {
    ctx.setState(patch({
      screens,
    }));
  }

  @Action(SetIsAceAllowed)
  setIsAceAllowed(ctx: StateContext<AppStateModel>, { isAceAllowed}: SetIsAceAllowed) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      isAceAllowed,
    });
  }
}
