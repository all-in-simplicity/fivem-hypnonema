import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import Screen from "../types/screen";

export interface IScreenState {
  screens?: Screen[];
}

const initialState: IScreenState = {
  screens: [],
};

export const screens = createSlice({
  name: "screens",
  initialState,
  reducers: {
    setScreens: (state, action: PayloadAction<Screen[]>) => {
      state.screens = action.payload;
    },
  },
});

export const { setScreens } = screens.actions;
export const { reducer: screensReducer } = screens;
export default screens.reducer;
