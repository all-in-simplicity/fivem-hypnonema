import * as React from "react";
import { FC, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Drawer from "@mui/material/Drawer";
import LiveTvIcon from "@mui/icons-material/LiveTv";
import List from "@mui/material/List";
import Divider from "@mui/material/Divider";

import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import InfoIcon from "@mui/icons-material/Info";

import ListIcon from "@mui/icons-material/List";
import { Badge, BadgeProps, ListItemButton, styled } from "@mui/material";

import { Link, To, useMatch, useResolvedPath } from "react-router-dom";
import Toolbar from "@mui/material/Toolbar";
import { useAppSelector } from "../../hooks/store";

const drawerWidth = 240;

interface LinkItemProps {
  to: To;
}

const StyledBadge = styled(Badge)<BadgeProps>(({ theme }) => ({
  "& .MuiBadge-badge": {
    right: -13,
    top: 13,
    padding: "0 4px",
  },
}));

const LinkItem: FC<LinkItemProps> = ({ children, to, ...props }) => {
  const resolved = useResolvedPath(to);
  const match = useMatch({ path: resolved.pathname, end: true });

  return (
    <ListItemButton
      selected={match !== null}
      component={Link}
      to={to}
      {...props}
    >
      {children}
    </ListItemButton>
  );
};

interface MenuProps {
  containerRef: React.RefObject<HTMLElement>;
}

const Menu: FC<MenuProps> = (props) => {
  const [height, setHeight] = useState(0);

  const [width, setWidth] = useState(drawerWidth);

  const isMenuOpen = useAppSelector((state) => state.ui.isMenuOpen);

  const duiStates = useAppSelector((state) => state.dui.duiStates);

  useEffect(() => {
    if (isMenuOpen) {
      if (props.containerRef?.current) {
        setHeight(props.containerRef.current.clientHeight);
        setWidth(drawerWidth);
      }
    } else {
      setHeight(0);
      setWidth(0);
    }
  }, [props.containerRef, isMenuOpen]);

  return (
    <Drawer
      sx={{
        position: "relative",
        marginRight: "auto",

        width: width,
        flexShrink: 0,
        "& .MuiDrawer-paper": {
          width: width,
          position: "absolute",
          boxSizing: "border-box",
          height: height,
          transition: "none !important",
          backgroundColor: "#282828 !important",
        },
        "& .MuiBackdrop-root": {
          display: "none",
        },
      }}
      variant="persistent"
      open={isMenuOpen}
    >
      <Toolbar />
      <Box sx={{ overflow: "auto" }}>
        <List>
          <LinkItem to="/play">
            <ListItemIcon>
              <LiveTvIcon />
            </ListItemIcon>
            <ListItemText primary="Play" />
          </LinkItem>
          <LinkItem to="/screens">
            <ListItemIcon>
              <ListIcon />
            </ListItemIcon>
            <ListItemText primary="Screens" />
          </LinkItem>
          <LinkItem to="/status">
            <StyledBadge badgeContent={duiStates?.length} color="primary">
              <ListItemIcon>
                <InfoIcon />
              </ListItemIcon>
              <ListItemText primary="Status" />
            </StyledBadge>
          </LinkItem>
        </List>
        <Divider />
      </Box>
    </Drawer>
  );
};

export default Menu;
