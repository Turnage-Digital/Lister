import { useMemo } from "react";

import { useQueries, UseQueryResult } from "@tanstack/react-query";

import { ListSearch, PagedList } from "../models";
import { pagedItemsQueryOptions } from "../query-options";

/**
 * Hook that accumulates items from page 0 up to the specified page
 * for mobile "Load More" functionality while maintaining URL state consistency.
 */
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
  // Generate queries for pages 0 through search.page
  const pageQueries = useMemo(() => {
    const queries = [];
    for (let page = 0; page <= search.page; page++) {
      const pageSearch = { ...search, page };
      queries.push(pagedItemsQueryOptions(pageSearch, listId));
    }
    return queries;
  }, [search, listId]);

  // Fetch all pages using useQueries
  const queryResults: UseQueryResult<PagedList>[] = useQueries({
    queries: pageQueries,
  });

  // Accumulate all items and compute state
  const {
    accumulatedItems,
    totalCount,
    isLoading,
    hasMoreItems,
    allQueriesSuccessful,
  } = useMemo(() => {
    const allItems: PagedList["items"] = [];
    let maxTotalCount = 0;
    let anyLoading = false;
    let allSuccessful = true;

    for (const result of queryResults) {
      if (result.isLoading) {
        anyLoading = true;
      }
      if (!result.isSuccess) {
        allSuccessful = false;
      }
      if (result.data) {
        allItems.push(...result.data.items);
        maxTotalCount = Math.max(maxTotalCount, result.data.count);
      }
    }

    const hasMore = allItems.length < maxTotalCount;

    return {
      accumulatedItems: allItems,
      totalCount: maxTotalCount,
      isLoading: anyLoading,
      hasMoreItems: hasMore,
      allQueriesSuccessful: allSuccessful,
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
