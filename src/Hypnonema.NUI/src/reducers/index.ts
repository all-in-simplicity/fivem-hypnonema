import { configureStore } from "@reduxjs/toolkit";
import { IScreenState, screensReducer } from "./screens";
import { IUiState, uiReducer } from "./ui";
import { duiReducer, IDuiState } from "./dui";
import { IScheduleState, schedulesReducer } from "./schedules";

export interface IAppState {
  ui: IUiState;
  screens: IScreenState;
  dui: IDuiState;
  schedules: IScheduleState;
}

export const store = configureStore<IAppState>({
  reducer: {
    ui: uiReducer,
    screens: screensReducer,
    dui: duiReducer,
    schedules: schedulesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;

export type AppDispatch = typeof store.dispatch;
