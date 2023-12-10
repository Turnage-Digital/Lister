import { createBrowserRouter, redirect } from "react-router-dom";

import {
  DashboardPage,
  dashboardPageLoader,
  EditListPage,
  editListPageAction,
  editListPageLoader,
  Layout,
  layoutLoader,
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
        Component: DashboardPage,
        loader: dashboardPageLoader,
      },
      {
        path: "/create-list",
        Component: EditListPage,
        loader: editListPageLoader,
        action: editListPageAction,
      },
      {
        path: ":id/edit-list",
        Component: EditListPage,
        loader: editListPageLoader,
        action: editListPageAction,
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
