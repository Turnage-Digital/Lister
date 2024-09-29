import React from "react";
import {
  CssBaseline,
  StyledEngineProvider,
  ThemeProvider,
} from "@mui/material";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createRouter, RouterProvider } from "@tanstack/react-router";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { TanStackRouterDevtools } from "@tanstack/router-devtools";
import { createRoot } from "react-dom/client";

import { Loading, SideDrawerProvider } from "./components";
import { routeTree } from "./routeTree.gen";
import theme from "./theme";

export const queryClient = new QueryClient();

export const router = createRouter({
  routeTree,
  defaultPendingComponent: Loading,
  context: { queryClient },
  defaultPreload: "intent",
  defaultPreloadStaleTime: 0,
});

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

const rootElement = document.getElementById("root");
const root = createRoot(rootElement!);

root.render(
  <StyledEngineProvider injectFirst>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <SideDrawerProvider>
          <QueryClientProvider client={queryClient}>
            <RouterProvider router={router} />
            <TanStackRouterDevtools router={router} />
            <ReactQueryDevtools />
          </QueryClientProvider>
        </SideDrawerProvider>
      </LocalizationProvider>
    </ThemeProvider>
  </StyledEngineProvider>
);
