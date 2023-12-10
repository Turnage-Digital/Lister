import { createBrowserRouter, redirect } from "react-router-dom";

import {
  ListsPage,
  listsPageLoader,
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
    Component: Layout,
    loader: layoutLoader,
    children: [
      {
        path: "/",
        Component: ListsPage,
        loader: listsPageLoader,
        children: [
          {
            path: "/:id",
          },
        ],
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
