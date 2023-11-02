import { createBrowserRouter } from "react-router-dom";

import Shell from "./shell";
import {
  listDefsEditPageAction,
  listDefsEditPageLoader,
  ThingDefsEditPage,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    Component: Shell,
    children: [
      {
        path: "list-defs/create",
        Component: ThingDefsEditPage,
        loader: listDefsEditPageLoader,
        action: listDefsEditPageAction,
      },
    ],
  },
]);

export default router;
