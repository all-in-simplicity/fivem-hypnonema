import { FC } from "react";
import { Alert, Snackbar as MaterialSnackbar } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../hooks/store";
import {
  setSnackbar,
  SnackbarSeverity,
  SnackbarState,
} from "../../reducers/ui";

export const Snackbar: FC = () => {
  const snackbar: SnackbarState = useAppSelector((state) => state.ui.snackbar);

  const dispatch = useAppDispatch();

  const handleClose = () => {
    dispatch(
      setSnackbar({
        isVisible: false,
        message: "",
        type: SnackbarSeverity.Success,
      })
    );
  };

  return (
    <div>
      <MaterialSnackbar
        open={snackbar.isVisible}
        autoHideDuration={4000}
        onClose={handleClose}
      >
        <Alert
          onClose={handleClose}
          severity={snackbar.type}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </MaterialSnackbar>
    </div>
  );
};
