import { createBrowserRouter } from "react-router-dom";

import Shell from "./shell";
import {
  CreateListPage,
  createListPageAction,
  createListPageLoader,
  MainPage,
  mainPageLoader,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Shell,
    children: [
      {
        index: true,
        Component: MainPage,
        loader: mainPageLoader,
      },
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
