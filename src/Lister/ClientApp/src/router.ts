import { createBrowserRouter, redirect } from "react-router-dom";

import {
  CreateListPage,
  createListPageAction,
  createListPageLoader,
  Layout,
  layoutLoader,
  MainPage,
  mainPageLoader,
  SignInPage,
  signInPageAction,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Layout,
    loader: layoutLoader,
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
