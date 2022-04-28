import React from "react";
import ReactDOM from "react-dom";

import { App } from "containers/App";
import { HashRouter, Route, Routes } from "react-router-dom";
import {
  createTheme,
  CssBaseline,
  ThemeOptions,
  ThemeProvider,
} from "@mui/material";
import { NuiProvider } from "fivem-nui-react-lib";

import { Provider } from "react-redux";
import { store } from "reducers";

import { ScreensPage } from "./containers/ScreensPage";
import { EditScreenPage } from "./containers/EditScreenPage";
import { CreateScreenPage } from "./containers/CreateScreenPage";
import { PlayPage } from "./containers/PlayPage";
import { StatusPage } from "./containers/StatusPage";

const theme: ThemeOptions = createTheme({
  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundColor: "unset !important",
        },
      },
    },
  },
  palette: {
    mode: "dark",
    background: {
      default: "transparent",
    },
    primary: {
      main: "#5f0877",
      contrastText: "#fbf8f8",
    },
    secondary: {
      main: "#d83695",
    },
  },
});

const Layout = () => (
  <React.Fragment>
    <CssBaseline />
    <App />
  </React.Fragment>
);

const resourceName = window.location.hostname;

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <NuiProvider resource={resourceName}>
        <ThemeProvider theme={theme}>
          <HashRouter>
            <Routes>
              <Route path="/" element={<Layout />}>
                <Route index element={<PlayPage />} />
                <Route path="/play" element={<PlayPage />} />
                <Route path="/screens" element={<ScreensPage />} />
                <Route path="/screen/:screenId" element={<EditScreenPage />} />
                <Route path="/create-screen" element={<CreateScreenPage />} />
                <Route path="/status" element={<StatusPage />} />
              </Route>
            </Routes>
          </HashRouter>
        </ThemeProvider>
      </NuiProvider>
    </Provider>
  </React.StrictMode>,
  document.getElementById("root")
);
