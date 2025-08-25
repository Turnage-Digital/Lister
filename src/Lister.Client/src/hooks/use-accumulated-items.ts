import { useMemo } from "react";

import { useQueries, UseQueryResult } from "@tanstack/react-query";

import { ListSearch, PagedList } from "../models";
import { pagedItemsQueryOptions } from "../query-options";

export const useAccumulatedItems = (
  search: ListSearch,
  listId: string,
): {
  accumulatedItems: PagedList["items"];
  totalCount: number;
  isLoading: boolean;
  hasMoreItems: boolean;
  allQueriesSuccessful: boolean;
} => {
  const pageQueries = useMemo(() => {
    const queries = [];
    for (let page = 0; page <= search.page; page++) {
      const pageSearch = { ...search, page };
      queries.push(pagedItemsQueryOptions(pageSearch, listId));
    }
    return queries;
  }, [search, listId]);

  const queryResults: UseQueryResult<PagedList>[] = useQueries({
    queries: pageQueries,
  });

  const {
    accumulatedItems,
    totalCount,
    isLoading,
    hasMoreItems,
    allQueriesSuccessful,
  } = useMemo(() => {
    const accumulatedItems: PagedList["items"] = [];
    let totalCount = 0;
    let isLoading = false;
    let allQueriesSuccessful = true;

    for (const result of queryResults) {
      if (result.isLoading) {
        isLoading = true;
      }
      if (!result.isSuccess) {
        allQueriesSuccessful = false;
      }
      if (result.data) {
        accumulatedItems.push(...result.data.items);
        totalCount = Math.max(totalCount, result.data.count);
      }
    }

    const hasMoreItems = accumulatedItems.length < totalCount;

    return {
      accumulatedItems,
      totalCount,
      isLoading,
      hasMoreItems,
      allQueriesSuccessful,
    };
  }, [queryResults]);

  return {
    accumulatedItems,
    totalCount,
    isLoading,
    hasMoreItems,
    allQueriesSuccessful,
  };
};
