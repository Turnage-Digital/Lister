import * as React from "react";

import { useInfiniteQuery } from "@tanstack/react-query";

import HistoryDrawer from "./history-drawer";
import { listHistoryInfiniteQueryOptions } from "../../query-options";

interface ListHistoryDrawerProps {
  listId: string;
}

const ListHistoryDrawer = ({ listId }: ListHistoryDrawerProps) => {
  const query = useInfiniteQuery(listHistoryInfiniteQueryOptions(listId));

  return <HistoryDrawer subtitle="Latest list updates" query={query} />;
};

export default ListHistoryDrawer;
