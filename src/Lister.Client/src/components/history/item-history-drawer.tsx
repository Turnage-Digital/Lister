import * as React from "react";

import HistoryDrawer from "./history-drawer";
import { fetchItemHistory } from "../../api/history";

interface ItemHistoryDrawerProps {
  listId: string;
  itemId: number;
}

const ItemHistoryDrawer = ({ listId, itemId }: ItemHistoryDrawerProps) => (
  <HistoryDrawer
    subtitle="Status changes and updates for this item."
    queryKey={["item-history", listId, itemId]}
    fetchPage={(page) => fetchItemHistory(listId, itemId, page)}
  />
);

export default ItemHistoryDrawer;
