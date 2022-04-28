import { configureStore } from "@reduxjs/toolkit";
import { IScreenState, screensReducer } from "./screens";
import { IUiState, uiReducer } from "./ui";
import { duiReducer, IDuiState } from "./dui";

export interface IAppState {
  ui: IUiState;
  screens: IScreenState;
  dui: IDuiState;
}

export const store = configureStore<IAppState>({
  reducer: {
    ui: uiReducer,
    screens: screensReducer,
    dui: duiReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;

export type AppDispatch = typeof store.dispatch;
