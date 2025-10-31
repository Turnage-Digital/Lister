import * as React from "react";

import RestoreIcon from "@mui/icons-material/Restore";
import {
  Box,
  Button,
  Chip,
  CircularProgress,
  List,
  ListItem,
  Skeleton,
  Stack,
  Typography,
} from "@mui/material";
import { alpha } from "@mui/material/styles";
import {
  type InfiniteData,
  type UseInfiniteQueryResult,
} from "@tanstack/react-query";

import { HistoryPage } from "../../models";
import {
  SideDrawerContainer,
  SideDrawerContent,
  SideDrawerFooter,
  SideDrawerHeader,
} from "../side-drawer";

const formatTimestamp = (isoString: string | undefined) => {
  if (isoString === undefined || isoString === "") {
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
  query: UseInfiniteQueryResult<HistoryPage>;
}

const HISTORY_SKELETON_KEYS = [
  "history-skeleton-1",
  "history-skeleton-2",
  "history-skeleton-3",
  "history-skeleton-4",
] as const;

const HistoryListSkeleton = () => (
  <List disablePadding sx={{ flex: 1 }}>
    {HISTORY_SKELETON_KEYS.map((skeletonKey, index) => {
      const hasSkeletonConnector = index < HISTORY_SKELETON_KEYS.length - 1;

      const connector = hasSkeletonConnector ? (
        <Box
          sx={{
            flex: 1,
            width: 2,
            backgroundColor: (theme) => theme.palette.divider,
            mt: 1,
          }}
        />
      ) : null;

      return (
        <ListItem
          key={skeletonKey}
          disableGutters
          sx={{ py: 1, alignItems: "stretch" }}
        >
          <Stack direction="row" spacing={2} sx={{ width: "100%" }}>
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                minWidth: 32,
              }}
            >
              <Skeleton variant="circular" width={32} height={32} />
              {connector}
            </Box>
            <Stack
              spacing={1}
              sx={{
                flex: 1,
                px: "1rem",
                py: "0.9rem",
                borderRadius: (theme) => theme.shape.borderRadius,
                border: (theme) => `1px solid ${theme.palette.divider}`,
                backgroundColor: (theme) => theme.palette.background.paper,
              }}
            >
              <Skeleton variant="text" width="35%" />
              <Skeleton variant="text" width="55%" />
              <Skeleton variant="text" width="25%" />
            </Stack>
          </Stack>
        </ListItem>
      );
    })}
  </List>
);

const HistoryDrawer = ({ subtitle, query }: HistoryDrawerProps) => {
  const infiniteData = query.data as InfiniteData<HistoryPage> | undefined;

  const pages = React.useMemo<HistoryPage[]>(() => {
    if (infiniteData === undefined) {
      return [];
    }
    return [...infiniteData.pages];
  }, [infiniteData]);

  const entries = React.useMemo(() => {
    return pages.flatMap((page) => page.items);
  }, [pages]);

  const totalEntries = pages.length > 0 ? pages[0]?.total : undefined;

  const isInitialLoading =
    query.isPending || (query.isFetching && entries.length === 0);

  const loadMoreIcon = query.isFetchingNextPage ? (
    <CircularProgress size={16} />
  ) : undefined;

  let loadMoreNode: React.ReactNode;
  if (isInitialLoading) {
    loadMoreNode = <CircularProgress size={20} />;
  } else if (query.hasNextPage) {
    loadMoreNode = (
      <Button
        variant="outlined"
        size="small"
        onClick={() => query.fetchNextPage()}
        endIcon={loadMoreIcon}
        disabled={query.isFetchingNextPage}
      >
        Load more
      </Button>
    );
  } else {
    loadMoreNode = (
      <Typography variant="caption" color="text.secondary">
        All history loaded
      </Typography>
    );
  }

  const listContent =
    entries.length > 0 ? (
      <List disablePadding sx={{ flex: 1 }}>
        {entries.map((entry, index) => {
          const entryKey = `${entry.on}-${entry.type}-${entry.by ?? "unknown"}`;
          const hasTrailingConnector = index < entries.length - 1;

          const performerLine = entry.by ? (
            <Typography variant="body2" color="text.secondary">
              Performed by {entry.by}
            </Typography>
          ) : null;

          const entryConnector = hasTrailingConnector ? (
            <Box
              sx={{
                flex: 1,
                width: 2,
                backgroundColor: (theme) => theme.palette.divider,
                mt: 1,
                borderRadius: (theme) => theme.shape.borderRadius,
              }}
            />
          ) : null;

          return (
            <ListItem
              key={entryKey}
              disableGutters
              sx={{ py: 1, alignItems: "stretch" }}
            >
              <Stack direction="row" spacing={2} sx={{ width: "100%" }}>
                <Box
                  sx={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    minWidth: 32,
                  }}
                >
                  <Box
                    sx={{
                      width: 32,
                      height: 32,
                      borderRadius: "50%",
                      backgroundColor: "primary.main",
                      color: "primary.contrastText",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <RestoreIcon fontSize="small" />
                  </Box>
                  {entryConnector}
                </Box>
                <Stack
                  spacing={0.5}
                  sx={{
                    flex: 1,
                    px: "1rem",
                    py: "0.9rem",
                    borderRadius: (theme) => theme.shape.borderRadius,
                    border: (theme) => `1px solid ${theme.palette.divider}`,
                    backgroundColor: (theme) => theme.palette.background.paper,
                  }}
                >
                  <Typography variant="caption" color="text.secondary">
                    {formatTimestamp(entry.on)}
                  </Typography>
                  <Typography variant="subtitle2">{entry.type}</Typography>
                  {performerLine}
                </Stack>
              </Stack>
            </ListItem>
          );
        })}
      </List>
    ) : (
      <Box
        sx={{
          py: 8,
          textAlign: "center",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: 2,
        }}
      >
        <Box
          sx={{
            width: 56,
            height: 56,
            borderRadius: "50%",
            backgroundColor: (theme) => theme.palette.grey[100],
            color: (theme) => theme.palette.primary.dark,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <RestoreIcon />
        </Box>
        <Typography variant="subtitle1">No history yet</Typography>
        <Typography variant="body2" color="text.secondary">
          Activity on your lists will start appearing here.
        </Typography>
      </Box>
    );

  const contentNode = isInitialLoading ? <HistoryListSkeleton /> : listContent;

  let headerActions: React.ReactNode;
  if (typeof totalEntries === "number") {
    headerActions = (
      <Chip
        size="small"
        label={`${totalEntries} entr${totalEntries === 1 ? "y" : "ies"}`}
        color="default"
      />
    );
  }

  const hasNextPage = Boolean(query.hasNextPage);
  let footerJustify: "center" | "flex-end" = "center";
  if (hasNextPage) {
    footerJustify = isInitialLoading ? "center" : "flex-end";
  }

  return (
    <SideDrawerContainer>
      <SideDrawerHeader subtitle={subtitle} actions={headerActions} />
      <SideDrawerContent>
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            flex: 1,
            p: 2,
            gap: 2,
          }}
        >
          <Box
            sx={{
              flex: 1,
              overflowY: "auto",
              display: "flex",
              flexDirection: "column",
            }}
          >
            {contentNode}
          </Box>
        </Box>
      </SideDrawerContent>
      <SideDrawerFooter>
        <Box
          sx={{
            display: "flex",
            width: "100%",
            justifyContent: footerJustify,
          }}
        >
          {loadMoreNode}
        </Box>
      </SideDrawerFooter>
    </SideDrawerContainer>
  );
};

export default HistoryDrawer;
