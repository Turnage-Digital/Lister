import { createBrowserRouter } from "react-router-dom";

import {
  EditListItemPage,
  editListItemPageAction,
  EditListPage,
  editListPageAction,
  ListIdPage,
  ListItemIdPage,
  ListsPage,
  Root,
} from "./pages";

const router = createBrowserRouter([
  {
    id: "root",
    path: "/",
    Component: Root,
    children: [
      {
        path: "/",
        Component: ListsPage,
      },
      {
        path: "/:listId",
        Component: ListIdPage,
      },
      {
        path: "/create",
        Component: EditListPage,
        action: editListPageAction,
      },
      {
        path: "/:listId/items/:itemId",
        Component: ListItemIdPage,
      },
      {
        path: "/:listId/items/create",
        Component: EditListItemPage,
        action: editListItemPageAction,
      },
    ],
  },
]);

export default router;
