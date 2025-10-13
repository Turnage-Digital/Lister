import * as React from "react";

import HistoryDrawer from "./history-drawer";
import { fetchListHistory } from "../../api/history";

interface ListHistoryDrawerProps {
  listId: string;
}

const ListHistoryDrawer = ({ listId }: ListHistoryDrawerProps) => (
  <HistoryDrawer
    subtitle="Latest changes to this list."
    queryKey={["list-history", listId]}
    fetchPage={(page) => fetchListHistory(listId, page)}
  />
);

export default ListHistoryDrawer;
