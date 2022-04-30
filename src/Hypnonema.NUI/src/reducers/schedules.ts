import Schedule from "../types/schedule";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface IScheduleState {
  schedules?: Schedule[];
}

const initialState: IScheduleState = {
  schedules: [],
};

export const schedules = createSlice({
  name: "schedules",
  initialState,
  reducers: {
    setSchedules: (state, action: PayloadAction<Schedule[]>) => {
      state.schedules = action.payload;
    },
  },
});

export const { setSchedules } = schedules.actions;
export const { reducer: schedulesReducer } = schedules;
export default schedules.reducer;
