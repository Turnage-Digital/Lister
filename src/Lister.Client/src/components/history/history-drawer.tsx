import * as React from "react";

import CloseIcon from "@mui/icons-material/Close";
import RestoreIcon from "@mui/icons-material/Restore";
import {
  Box,
  Button,
  CircularProgress,
  IconButton,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { useInfiniteQuery, type InfiniteData } from "@tanstack/react-query";

import { HistoryPage } from "../../models";
import SideDrawerContent from "../side-drawer/side-drawer-content";
import useSideDrawer from "../side-drawer/use-side-drawer";

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
  title: string;
  subtitle?: string;
  queryKey: ReadonlyArray<unknown>;
  fetchPage: (pageParam: number) => Promise<HistoryPage>;
}

const HistoryDrawer = ({
  title,
  subtitle,
  queryKey,
  fetchPage,
}: HistoryDrawerProps) => {
  const { closeDrawer } = useSideDrawer();

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

  const loadMoreLabel = infiniteQuery.hasNextPage
    ? "Load more"
    : "All history loaded";

  const loadMoreIcon = infiniteQuery.isFetchingNextPage ? (
    <CircularProgress size={16} />
  ) : undefined;

  const subtitleNode = subtitle ? (
    <Typography variant="body2" color="text.secondary">
      {subtitle}
    </Typography>
  ) : null;

  const listContent = hasEntries ? (
    <List disablePadding sx={{ px: 2 }}>
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
    <SideDrawerContent>
      <Stack spacing={2} sx={{ p: 2 }}>
        <Stack direction="row" alignItems="center" spacing={1}>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            {title}
          </Typography>
          <IconButton aria-label="Close" onClick={closeDrawer}>
            <CloseIcon />
          </IconButton>
        </Stack>
        {subtitleNode}
      </Stack>

      <Box sx={{ flex: 1, overflowY: "auto" }}>{listContent}</Box>

      <Box
        sx={{
          p: 2,
          display: "flex",
          justifyContent: "flex-end",
        }}
      >
        <Button
          variant="outlined"
          size="small"
          onClick={() => infiniteQuery.fetchNextPage()}
          disabled={!infiniteQuery.hasNextPage}
          endIcon={loadMoreIcon}
        >
          {loadMoreLabel}
        </Button>
      </Box>
    </SideDrawerContent>
  );
};

export default HistoryDrawer;
