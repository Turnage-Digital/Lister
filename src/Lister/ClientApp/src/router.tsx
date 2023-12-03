import { createBrowserRouter, redirect } from "react-router-dom";

import Shell, { shellLoader } from "./shell";
import {
  CreateListPage,
  createListPageAction,
  createListPageLoader,
  MainPage,
  mainPageLoader,
  SignInPage,
  signInPageAction,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Shell,
    loader: shellLoader,
    children: [
      {
        index: true,
        Component: MainPage,
        loader: mainPageLoader,
      },
      {
        path: "/create",
        Component: CreateListPage,
        loader: createListPageLoader,
        action: createListPageAction,
      },
    ],
  },
  {
    path: "/sign-in",
    Component: SignInPage,
    action: signInPageAction,
  },
  {
    path: "/sign-out",
    action: async () => {
      await fetch("/api/users/sign-out", { method: "POST" });
      return redirect("/");
    },
  },
]);

export default router;
