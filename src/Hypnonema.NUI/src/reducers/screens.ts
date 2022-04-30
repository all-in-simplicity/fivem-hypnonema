import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import Screen from "../types/screen";

export interface IScreenState {
  screens?: Screen[];
}

const initialState: IScreenState = {
  screens: [
    {
      id: 5,
      name: "test",
      is3DRendered: false,
      maxRenderDistance: 400,
      targetSettings: { renderTargetName: "blab", modelName: "test" },
      browserSettings: {
        globalVolume: 100,
        soundMinDistance: 20,
        soundMaxDistance: 20,
        soundAttenuation: 30,
        is3DAudioEnabled: false,
      },
      alwaysOn: false,
    },
  ],
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
