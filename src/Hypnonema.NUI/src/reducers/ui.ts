import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export enum SnackbarSeverity {
  Success = "success",
  Error = "error",
  Warning = "warning",
  Info = "info",
}

export interface SnackbarState {
  isVisible: boolean;
  message: string;
  type: SnackbarSeverity;
}

export interface IUiState {
  isAppVisible: boolean;
  isMenuOpen: boolean;
  snackbar: SnackbarState;
}

const initialState: IUiState = {
  isAppVisible: true,
  isMenuOpen: true,
  snackbar: {
    isVisible: false,
    message: "",
    type: SnackbarSeverity.Success,
  },
};

export const ui = createSlice({
  name: "ui",
  initialState,
  reducers: {
    setIsAppVisible: (state, action: PayloadAction<boolean>) => {
      state.isAppVisible = action.payload;
    },
    setIsMenuOpen: (state, action: PayloadAction<boolean>) => {
      state.isMenuOpen = action.payload;
    },
    setSnackbar: (state, action: PayloadAction<SnackbarState>) => {
      state.snackbar = action.payload;
    },
  },
});

export const { setIsAppVisible, setIsMenuOpen, setSnackbar } = ui.actions;
export const { reducer: uiReducer } = ui;
