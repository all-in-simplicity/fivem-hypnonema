import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { DuiState } from "../types/duiState";

export interface IDuiState {
  duiStates?: DuiState[];
}

const initialState: IDuiState = {
  duiStates: [],
};

export const duiStates = createSlice({
  name: "duiStates",
  initialState,
  reducers: {
    setDuiStates: (state, action: PayloadAction<DuiState[]>) => {
      state.duiStates = action.payload;
    },
  },
});

export const { setDuiStates } = duiStates.actions;
export const { reducer: duiReducer } = duiStates;
