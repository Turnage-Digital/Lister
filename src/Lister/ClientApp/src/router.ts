import { createBrowserRouter } from "react-router-dom";

import Shell from "./shell";
import { EditListPage, editListPageAction, editListPageLoader } from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Shell,
    children: [
      {
        path: "lists/create",
        Component: EditListPage,
        loader: editListPageLoader,
        action: editListPageAction,
      },
    ],
  },
]);

export default router;
