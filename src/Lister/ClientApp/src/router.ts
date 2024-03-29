import { createBrowserRouter, redirect } from "react-router-dom";

import {
  EditListItemPage,
  editListItemPageAction,
  editListItemPageLoader,
  EditListPage,
  editListPageAction,
  editListPageLoader,
  ListIdPage,
  listIdPageLoader,
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
    path: "/",
    Component: Root,
    loader: rootLoader,
    children: [
      {
        path: "/",
        Component: ListsPage,
        loader: listsPageLoader,
        children: [
          {
            path: "/:listId",
            Component: ListIdPage,
            loader: listIdPageLoader,
          },
        ],
      },
      {
        path: "/create",
        Component: EditListPage,
        loader: editListPageLoader,
        action: editListPageAction,
      },
      {
        path: "/:listId/items/create",
        Component: EditListItemPage,
        loader: editListItemPageLoader,
        action: editListItemPageAction,
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
