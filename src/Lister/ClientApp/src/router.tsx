import { createBrowserRouter } from "react-router-dom";
import React from "react";

import Shell from "./shell";
import {
  CreateListPage,
  createListPageAction,
  createListPageLoader,
  MainPage,
  mainPageLoader,
  SignInPage,
  signInPageAction,
} from "./pages";
import { AuthProvider } from "./auth";

const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <AuthProvider>
        <Shell />
      </AuthProvider>
    ),
    children: [
      {
        index: true,
        element: <MainPage />,
        loader: mainPageLoader,
      },
      {
        path: "create",
        element: <CreateListPage />,
        loader: createListPageLoader,
        action: createListPageAction,
      },
    ],
  },
  {
    path: "sign-in",
    element: <SignInPage />,
    action: signInPageAction,
  },
]);

export default router;
