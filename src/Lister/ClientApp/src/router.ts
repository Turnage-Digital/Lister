import { createBrowserRouter, redirect } from "react-router-dom";

import {
  EditListPage,
  editListPageAction,
  editListPageLoader,
  IdPage,
  idPageLoader,
  ListsPage,
  listsPageLoader,
  Root,
  rootLoader,
  SignInPage,
  signInPageAction,
} from "./pages";

const router = createBrowserRouter([
  {
    id: "root",
    Component: Root,
    loader: rootLoader,
    children: [
      {
        path: "/",
        Component: ListsPage,
        loader: listsPageLoader,
        children: [
          {
            path: "/:id",
            Component: IdPage,
            loader: idPageLoader,
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
