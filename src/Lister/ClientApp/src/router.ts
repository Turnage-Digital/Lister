import { createBrowserRouter } from "react-router-dom";

import Shell from "./shell";
import {
  CreateListPage,
  createListPageAction,
  createListPageLoader,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Shell,
    children: [
      {
        path: "create",
        Component: CreateListPage,
        loader: createListPageLoader,
        action: createListPageAction,
      },
    ],
  },
]);

export default router;
