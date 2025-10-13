import * as React from "react";

import CloseIcon from "@mui/icons-material/Close";
import FilterListIcon from "@mui/icons-material/FilterList";
import MarkEmailReadIcon from "@mui/icons-material/MarkEmailRead";
import NotificationsActiveIcon from "@mui/icons-material/NotificationsActive";
import {
  Box,
  Button,
  Chip,
  CircularProgress,
  Divider,
  IconButton,
  List,
  ListItemButton,
  Stack,
  ToggleButton,
  ToggleButtonGroup,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  type InfiniteData,
  useInfiniteQuery,
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";

import {
  fetchNotifications,
  markAllNotificationsRead,
  markNotificationRead,
} from "../../api/notifications";
import {
  NotificationListItem,
  NotificationListPage,
  NotificationsSearch,
} from "../../models";
import {
  notificationDetailsQueryOptions,
  unreadCountQueryOptions,
} from "../../query-options";
import SideDrawerContent from "../side-drawer/side-drawer-content";
import useSideDrawer from "../side-drawer/use-side-drawer";

const PAGE_SIZE = 20;

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

const buildSearch = (filter: "all" | "unread"): NotificationsSearch => ({
  unread: filter === "unread" ? true : undefined,
  pageSize: PAGE_SIZE,
});

const NotificationsDrawer = () => {
  const queryClient = useQueryClient();
  const { closeDrawer, openDrawer } = useSideDrawer();
  const [filter, setFilter] = React.useState<"all" | "unread">("all");

  const { data: unreadTotal } = useSuspenseQuery(unreadCountQueryOptions());

  const search = React.useMemo(() => buildSearch(filter), [filter]);

  const infiniteQuery = useInfiniteQuery({
    queryKey: ["notifications", search],
    initialPageParam: 0,
    queryFn: ({ pageParam }) =>
      fetchNotifications({ ...search, page: pageParam }),
    getNextPageParam: (lastPage) => {
      const totalPages = Math.ceil(lastPage.total / lastPage.pageSize);
      const nextPage = lastPage.page + 1;
      return nextPage < totalPages ? nextPage : undefined;
    },
  });

  const infiniteData = infiniteQuery.data as
    | InfiniteData<NotificationListPage>
    | undefined;

  const pages = React.useMemo<NotificationListPage[]>(() => {
    if (!infiniteData) {
      return [];
    }
    return [...infiniteData.pages];
  }, [infiniteData]);

  const notifications = React.useMemo<NotificationListItem[]>(() => {
    return pages.flatMap((page) => {
      if (!Array.isArray(page.items)) {
        return [] as NotificationListItem[];
      }

      return page.items.filter((item): item is NotificationListItem =>
        Boolean(item),
      );
    });
  }, [pages]);

  const totalAvailable = React.useMemo(() => {
    const lastPage = pages.length > 0 ? pages[pages.length - 1] : undefined;
    return lastPage?.total ?? notifications.length;
  }, [pages, notifications.length]);

  const invalidateNotifications = React.useCallback(() => {
    queryClient.invalidateQueries({
      queryKey: ["notifications"],
      exact: false,
    });
    queryClient.invalidateQueries({
      queryKey: ["notifications-unread-count"],
      exact: false,
    });
  }, [queryClient]);

  const markSingleMutation = useMutation({
    mutationFn: markNotificationRead,
    onSuccess: () => invalidateNotifications(),
  });

  const markAllMutation = useMutation({
    mutationFn: markAllNotificationsRead,
    onSuccess: () => invalidateNotifications(),
  });

  const handleMarkAll = () => {
    if (unreadTotal > 0 && !markAllMutation.isPending) {
      markAllMutation.mutate();
    }
  };

  const handleSelectFilter = (
    _: React.SyntheticEvent,
    next: "all" | "unread" | null,
  ) => {
    if (next) {
      setFilter(next);
    }
  };

  const handleLoadMore = () => {
    if (infiniteQuery.hasNextPage) {
      infiniteQuery.fetchNextPage();
    }
  };

  const handleOpenDetails = (id: string) => {
    openDrawer(
      "Notification",
      <NotificationDetailsView id={id} onClose={closeDrawer} />,
    );
  };

  const handleMarkSingle = (
    event: React.MouseEvent,
    notificationId: string,
  ) => {
    event.stopPropagation();
    markSingleMutation.mutate(notificationId);
  };

  const emptyStateMessage =
    filter === "unread" ? "You're all caught up" : "No notifications yet";

  const emptyState = (
    <Box sx={{ py: 8, px: 2, textAlign: "center" }}>
      <NotificationsActiveIcon sx={{ fontSize: 48, color: "text.disabled" }} />
      <Typography variant="subtitle1" sx={{ mt: 2 }}>
        {emptyStateMessage}
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Notifications from your lists and items will appear here.
      </Typography>
    </Box>
  );

  const listItems = notifications.map((notification) => {
    const isUnread = !notification.isRead;
    const timestamp = formatTimestamp(notification.occurredOn);
    const timestampNode = timestamp ? (
      <Typography
        variant="caption"
        color="text.secondary"
        sx={{ ml: 1, whiteSpace: "nowrap" }}
      >
        {timestamp}
      </Typography>
    ) : undefined;

    const newBadge = isUnread ? (
      <Chip size="small" label="New" color="primary" variant="outlined" />
    ) : undefined;

    const titleWeight = isUnread ? 600 : 500;

    const markButton = isUnread ? (
      <Tooltip title="Mark as read">
        <IconButton
          edge="end"
          size="small"
          onClick={(event) => handleMarkSingle(event, notification.id)}
        >
          <MarkEmailReadIcon fontSize="small" />
        </IconButton>
      </Tooltip>
    ) : undefined;

    return (
      <ListItemButton
        key={notification.id}
        alignItems="flex-start"
        onClick={() => handleOpenDetails(notification.id)}
        sx={{
          mb: 1,
          borderRadius: 1,
          backgroundColor: isUnread ? "action.hover" : "transparent",
          border: (theme) =>
            isUnread
              ? `1px solid ${theme.palette.primary.light}`
              : `1px solid ${theme.palette.divider}`,
        }}
      >
        <Stack spacing={1} sx={{ flexGrow: 1, minWidth: 0 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            {newBadge}
            <Typography
              variant="subtitle2"
              fontWeight={titleWeight}
              noWrap
              sx={{ flexGrow: 1 }}
            >
              {notification.title}
            </Typography>
            {timestampNode}
          </Stack>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
          >
            {notification.body}
          </Typography>
        </Stack>
        {markButton}
      </ListItemButton>
    );
  });

  let content: React.ReactNode = emptyState;
  if (notifications.length > 0) {
    content = (
      <List disablePadding sx={{ px: 2, py: 1 }}>
        {listItems}
      </List>
    );
  }

  const loadMoreLabel = infiniteQuery.hasNextPage
    ? "Load more"
    : "All caught up";

  const loadMoreIcon = infiniteQuery.isFetchingNextPage ? (
    <CircularProgress size={16} />
  ) : undefined;

  const loadMoreDisabled = !infiniteQuery.hasNextPage;

  const chipColor = unreadTotal > 0 ? "primary" : "default";
  const chipVariant = unreadTotal > 0 ? "outlined" : "filled";

  return (
    <SideDrawerContent>
      <Box
        sx={{
          p: 2,
          borderBottom: (theme) => `1px solid ${theme.palette.divider}`,
        }}
      >
        <Stack direction="row" alignItems="center" spacing={1}>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Notifications
          </Typography>
          <Button
            size="small"
            startIcon={<MarkEmailReadIcon fontSize="small" />}
            onClick={handleMarkAll}
            disabled={unreadTotal === 0 || markAllMutation.isPending}
          >
            Mark all read
          </Button>
          <IconButton aria-label="Close" onClick={closeDrawer}>
            <CloseIcon />
          </IconButton>
        </Stack>

        <Stack direction="row" spacing={1} alignItems="center" sx={{ mt: 2 }}>
          <Chip
            icon={<FilterListIcon fontSize="small" />}
            label={`${unreadTotal} unread`}
            size="small"
            color={chipColor}
            variant={chipVariant}
          />
          <ToggleButtonGroup
            size="small"
            exclusive
            value={filter}
            onChange={handleSelectFilter}
            aria-label="Notification filter"
          >
            <ToggleButton value="all">All</ToggleButton>
            <ToggleButton value="unread">Unread</ToggleButton>
          </ToggleButtonGroup>
        </Stack>
      </Box>

      <Box sx={{ flex: 1, overflowY: "auto" }}>{content}</Box>

      <Divider />
      <Box
        sx={{
          p: 2,
          display: "flex",
          justifyContent: "space-between",
          gap: 2,
          alignItems: "center",
        }}
      >
        <Typography variant="body2" color="text.secondary">
          Showing {notifications.length} of {totalAvailable}
        </Typography>
        <Button
          variant="outlined"
          size="small"
          onClick={handleLoadMore}
          disabled={loadMoreDisabled}
          endIcon={loadMoreIcon}
        >
          {loadMoreLabel}
        </Button>
      </Box>
    </SideDrawerContent>
  );
};

const NotificationDetailsView = ({
  id,
  onClose,
}: {
  id: string;
  onClose: () => void;
}) => {
  const queryClient = useQueryClient();
  const { data } = useSuspenseQuery(notificationDetailsQueryOptions(id));

  const invalidateNotifications = React.useCallback(() => {
    queryClient.invalidateQueries({
      queryKey: ["notifications"],
      exact: false,
    });
    queryClient.invalidateQueries({
      queryKey: ["notifications-unread-count"],
      exact: false,
    });
  }, [queryClient]);

  const markReadMutation = useMutation({
    mutationFn: () => markNotificationRead(id),
    onSuccess: () => invalidateNotifications(),
  });

  const metadataEntries = Object.entries(data.metadata ?? {});
  const historyEntries = data.history;
  const deliveryEntries = data.deliveryAttempts;
  const occurredOn =
    historyEntries.length > 0 ? historyEntries[0]?.on : undefined;
  const occurredOnLabel = formatTimestamp(occurredOn);
  const metadataSection =
    metadataEntries.length > 0 ? (
      <Box>
        <Typography variant="subtitle2" gutterBottom>
          Metadata
        </Typography>
        <Box
          component="dl"
          sx={{
            display: "grid",
            gridTemplateColumns: "max-content 1fr",
            columnGap: 2,
            rowGap: 1,
          }}
        >
          {metadataEntries.map(([key, value]) => (
            <React.Fragment key={key}>
              <Typography component="dt" variant="body2" color="text.secondary">
                {key}
              </Typography>
              <Typography component="dd" variant="body2">
                {String(value)}
              </Typography>
            </React.Fragment>
          ))}
        </Box>
      </Box>
    ) : null;

  const historySection =
    historyEntries.length > 0 ? (
      <Box>
        <Typography variant="subtitle2" gutterBottom>
          History
        </Typography>
        <Stack spacing={0.5}>
          {historyEntries.map((entry) => (
            <Typography
              key={`${entry.type}-${entry.on}`}
              variant="body2"
              color="text.secondary"
            >
              {formatTimestamp(entry.on)} — {entry.type}
            </Typography>
          ))}
        </Stack>
      </Box>
    ) : null;

  const deliverySection =
    deliveryEntries.length > 0 ? (
      <Box>
        <Typography variant="subtitle2" gutterBottom>
          Delivery attempts
        </Typography>
        <Stack spacing={0.5}>
          {deliveryEntries.map((attempt) => {
            const failureSuffix = attempt.failureReason
              ? ` (${attempt.failureReason})`
              : "";
            return (
              <Typography
                key={`${attempt.channel}-${attempt.attemptNumber}`}
                variant="body2"
                color="text.secondary"
              >
                {formatTimestamp(attempt.attemptedOn)} — {attempt.channel} →{" "}
                {attempt.status}
                {failureSuffix}
              </Typography>
            );
          })}
        </Stack>
      </Box>
    ) : null;

  const occurredOnNode = occurredOnLabel ? (
    <Typography variant="caption" color="text.secondary">
      {occurredOnLabel}
    </Typography>
  ) : null;

  const markReadLabel = data.isRead ? "Already read" : "Mark as read";

  return (
    <SideDrawerContent>
      <Box
        sx={{
          p: 2,
          borderBottom: (theme) => `1px solid ${theme.palette.divider}`,
        }}
      >
        <Stack direction="row" alignItems="center" spacing={1}>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            {data.title}
          </Typography>
          <IconButton aria-label="Close" onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        {occurredOnNode}
      </Box>

      <Box sx={{ p: 2, display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography variant="body1">{data.body}</Typography>
        {deliverySection}
        {metadataSection}
        {historySection}
        <Button
          variant="contained"
          startIcon={<MarkEmailReadIcon />}
          onClick={() => markReadMutation.mutate()}
          disabled={data.isRead || markReadMutation.isPending}
        >
          {markReadLabel}
        </Button>
      </Box>
    </SideDrawerContent>
  );
};

export default NotificationsDrawer;
