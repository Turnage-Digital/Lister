import { createBrowserRouter } from "react-router-dom";

import {
  EditListItemPage,
  EditListPage,
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
      },
      {
        path: "/:listId/items/:itemId",
        Component: ListItemIdPage,
      },
      {
        path: "/:listId/items/create",
        Component: EditListItemPage,
      },
    ],
  },
]);

export default router;
