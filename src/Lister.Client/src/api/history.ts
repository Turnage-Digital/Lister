import { HistoryPage } from "../models";

export const fetchListHistory = async (
  listId: string,
  page = 0,
  pageSize = 20,
): Promise<HistoryPage> => {
  const response = await fetch(
    `/api/lists/${listId}/history?page=${page}&pageSize=${pageSize}`,
  );
  if (!response.ok) {
    const message = await response.text().catch(() => "Failed to load history");
    throw new Error(message);
  }
  return (await response.json()) as HistoryPage;
};

export const fetchItemHistory = async (
  listId: string,
  itemId: number,
  page = 0,
  pageSize = 20,
): Promise<HistoryPage> => {
  const response = await fetch(
    `/api/lists/${listId}/items/${itemId}/history?page=${page}&pageSize=${pageSize}`,
  );
  if (!response.ok) {
    const message = await response.text().catch(() => "Failed to load history");
    throw new Error(message);
  }
  return (await response.json()) as HistoryPage;
};
