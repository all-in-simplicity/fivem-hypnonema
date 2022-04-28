import { FC } from "react";
import { IconButton } from "@mui/material";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import CloseIcon from "@mui/icons-material/Close";
import MenuIcon from "@mui/icons-material/Menu";
import { useAppDispatch, useAppSelector } from "../../hooks/store";
import { setIsMenuOpen } from "../../reducers/ui";
import { useNuiRequest } from "fivem-nui-react-lib";

export const Header: FC = () => {
  const { send } = useNuiRequest();

  const dispatch = useAppDispatch();

  const isMenuOpen = useAppSelector((state) => state.ui.isMenuOpen);

  const onMenuButtonClick = () => {
    dispatch(setIsMenuOpen(!isMenuOpen));
  };

  const onCloseButtonClick = () => {
    send("hideUI", {}).then(() => {});
  };

  return (
    <AppBar
      sx={{
        backgroundColor: "#5f0877",
        zIndex: (theme) => theme.zIndex.drawer + 1,
      }}
      position="fixed"
    >
      <Toolbar variant="dense" sx={{ boxShadow: 5 }}>
        <IconButton edge="start" color="inherit" onClick={onMenuButtonClick}>
          <MenuIcon />
        </IconButton>
        <div id="appHeader" style={{ flexGrow: 1 }}>
          <Typography
            variant="h6"
            color="inherit"
            sx={{
              margin: "auto",
              textAlign: "center",
            }}
            component="div"
          >
            Hypnonema
          </Typography>
        </div>
        <IconButton edge="end" color="inherit" onClick={onCloseButtonClick}>
          <CloseIcon />
        </IconButton>
      </Toolbar>
    </AppBar>
  );
};
