import { createBrowserRouter, redirect } from "react-router-dom";

import {
  EditListItemPage,
  editListItemPageAction,
  EditListPage,
  editListPageAction,
  ListItemPage,
  ListPage,
  ListsPage,
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
      },
      {
        path: "/:listId",
        Component: ListPage,
      },
      {
        path: "/create",
        Component: EditListPage,
        action: editListPageAction,
      },
      {
        path: "/:listId/items/:itemId",
        Component: ListItemPage,
      },
      {
        path: "/:listId/items/create",
        Component: EditListItemPage,
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
      await fetch("/identity/logout", { method: "POST" });
      return redirect("/");
    },
  },
]);

export default router;
