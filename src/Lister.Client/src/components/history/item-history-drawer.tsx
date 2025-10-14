import * as React from "react";

import { useInfiniteQuery } from "@tanstack/react-query";

import HistoryDrawer from "./history-drawer";
import { itemHistoryInfiniteQueryOptions } from "../../query-options";

interface ItemHistoryDrawerProps {
  listId: string;
  itemId: number;
}

const ItemHistoryDrawer = ({ listId, itemId }: ItemHistoryDrawerProps) => {
  const query = useInfiniteQuery(
    itemHistoryInfiniteQueryOptions(listId, itemId),
  );

  return (
    <HistoryDrawer
      subtitle="Status changes and updates for this item."
      query={query}
    />
  );
};

export default ItemHistoryDrawer;
