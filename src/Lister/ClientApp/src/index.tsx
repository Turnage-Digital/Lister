import React from "react";
import {
  CssBaseline,
  StyledEngineProvider,
  ThemeProvider,
} from "@mui/material";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";

import router from "./router";
import theme from "./theme";
import { Loading, SideDrawerProvider } from "./components";
import { AuthProvider } from "./auth";

const rootElement = document.getElementById("root");
const root = createRoot(rootElement!);

root.render(
  <StyledEngineProvider injectFirst>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <AuthProvider>
          <SideDrawerProvider>
            <RouterProvider router={router} fallbackElement={<Loading />} />
          </SideDrawerProvider>
        </AuthProvider>
      </LocalizationProvider>
    </ThemeProvider>
  </StyledEngineProvider>
);
