import { createBrowserRouter } from "react-router-dom";

import {
  EditListItemPage,
  editListItemPageAction,
  editListItemPageLoader,
  EditListPage,
  editListPageAction,
  editListPageLoader,
  ListIdPage,
  listIdPageLoader,
  ListItemIdPage,
  listItemIdPageLoader,
  ListsPage,
  listsPageLoader,
  Root,
} from "./pages";

const router = createBrowserRouter([
  {
    id: "root",
    path: "/",
    Component: Root,
    // children: [
    //   {
    //     path: "/",
    //     Component: ListsPage,
    //     loader: listsPageLoader,
    //     children: [
    //       {
    //         path: "/:listId",
    //         Component: ListIdPage,
    //         loader: listIdPageLoader,
    //       },
    //     ],
    //   },
    //   {
    //     path: "/create",
    //     Component: EditListPage,
    //     loader: editListPageLoader,
    //     action: editListPageAction,
    //   },
    //   {
    //     path: "/:listId/items/create",
    //     Component: EditListItemPage,
    //     loader: editListItemPageLoader,
    //     action: editListItemPageAction,
    //   },
    //   {
    //     path: "/:listId/items/:itemId",
    //     Component: ListItemIdPage,
    //     loader: listItemIdPageLoader,
    //   },
    // ],
  },
]);

export default router;
