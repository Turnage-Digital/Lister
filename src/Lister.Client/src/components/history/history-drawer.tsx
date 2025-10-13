import * as React from "react";

import RestoreIcon from "@mui/icons-material/Restore";
import {
  Box,
  Button,
  CircularProgress,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Tooltip,
  Typography,
} from "@mui/material";
import { type InfiniteData, useInfiniteQuery } from "@tanstack/react-query";

import { HistoryPage } from "../../models";
import {
  SideDrawerContainer,
  SideDrawerContent,
  SideDrawerFooter,
  SideDrawerHeader,
} from "../side-drawer";

const formatTimestamp = (isoString: string | undefined) => {
  if (!isoString) {
    return "";
  }
  const date = new Date(isoString);
  if (Number.isNaN(date.getTime())) {
    return isoString;
  }
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
};

interface HistoryDrawerProps {
  subtitle?: string;
  queryKey: ReadonlyArray<unknown>;
  fetchPage: (pageParam: number) => Promise<HistoryPage>;
}

const HistoryDrawer = ({
  subtitle,
  queryKey,
  fetchPage,
}: HistoryDrawerProps) => {
  const infiniteQuery = useInfiniteQuery<
    HistoryPage,
    Error,
    HistoryPage,
    ReadonlyArray<unknown>,
    number
  >({
    queryKey,
    initialPageParam: 0,
    queryFn: ({ pageParam }) => fetchPage(pageParam),
    getNextPageParam: (lastPage) => {
      const totalPages = Math.ceil(lastPage.total / lastPage.pageSize);
      const next = lastPage.page + 1;
      return next < totalPages ? next : undefined;
    },
  });

  const infiniteData = infiniteQuery.data as
    | InfiniteData<HistoryPage>
    | undefined;

  const pages = React.useMemo<HistoryPage[]>(() => {
    if (!infiniteData) {
      return [];
    }
    return [...infiniteData.pages];
  }, [infiniteData]);

  const entries = React.useMemo(
    () => pages.flatMap((page) => page.items),
    [pages],
  );

  const hasEntries = entries.length > 0;

  const loadMoreIcon = infiniteQuery.isFetchingNextPage ? (
    <CircularProgress size={16} />
  ) : undefined;

  const loadMoreNode = infiniteQuery.hasNextPage ? (
    <Button
      variant="outlined"
      size="small"
      onClick={() => infiniteQuery.fetchNextPage()}
      endIcon={loadMoreIcon}
      disabled={infiniteQuery.isFetchingNextPage}
    >
      Load more
    </Button>
  ) : (
    <Typography variant="caption" color="text.secondary">
      All history loaded
    </Typography>
  );

  const subtitleNode = subtitle ? (
    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
      {subtitle}
    </Typography>
  ) : null;

  const listContent = hasEntries ? (
    <List disablePadding sx={{ flex: 1 }}>
      {entries.map((entry) => {
        const entryKey = `${entry.on}-${entry.type}-${entry.by ?? "unknown"}`;
        const byLine = entry.by ? (
          <Typography variant="caption" color="text.secondary">
            {entry.by}
          </Typography>
        ) : null;

        return (
          <ListItem key={entryKey} alignItems="flex-start">
            <ListItemAvatar>
              <Tooltip title={String(entry.type)}>
                <RestoreIcon color="action" />
              </Tooltip>
            </ListItemAvatar>
            <ListItemText
              primary={formatTimestamp(entry.on)}
              secondary={
                <>
                  <Typography variant="body2" color="text.primary">
                    {entry.type}
                  </Typography>
                  {byLine}
                </>
              }
            />
          </ListItem>
        );
      })}
    </List>
  ) : (
    <Box sx={{ py: 8, textAlign: "center" }}>
      <Typography variant="body2" color="text.secondary">
        No history entries yet.
      </Typography>
    </Box>
  );

  return (
    <SideDrawerContainer>
      <SideDrawerHeader />
      <SideDrawerContent>
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            flex: 1,
            px: 3,
            py: 3,
            gap: 2,
          }}
        >
          {subtitleNode}
          <Box
            sx={{
              flex: 1,
              overflowY: "auto",
              display: "flex",
              flexDirection: "column",
            }}
          >
            {listContent}
          </Box>
        </Box>
      </SideDrawerContent>
      <SideDrawerFooter>
        <Box
          sx={{
            display: "flex",
            width: "100%",
            justifyContent: infiniteQuery.hasNextPage ? "flex-end" : "center",
          }}
        >
          {loadMoreNode}
        </Box>
      </SideDrawerFooter>
    </SideDrawerContainer>
  );
};

export default HistoryDrawer;
