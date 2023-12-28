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

// https://localhost:3000/sign-in -> SignInPage
// https://localhost:3000/sign-out -> Root

// https://localhost:3000/1 -> IdPage
// https://localhost:3000/create -> EditListPage
// https://localhost:3000/1/edit -> EditListPage

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
            Component: IdPage,
            loader: idPageLoader,
          },
        ],
      },
      // {
      //   path: "/:listId/items/create",
      //   // Component: EditListPage,
      //   // loader: editListPageLoader,
      //   // action: editListPageAction,
      // },
      // {
      //   path: "/:listId/items/:itemId/edit",
      //   // Component: EditListPage,
      //   // loader: editListPageLoader,
      //   // action: editListPageAction,
      // },
      {
        path: "/create",
        Component: EditListPage,
        loader: editListPageLoader,
        action: editListPageAction,
      },
      // {
      //   path: "/:listId/edit",
      //   Component: EditListPage,
      //   loader: editListPageLoader,
      //   action: editListPageAction,
      // },
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
