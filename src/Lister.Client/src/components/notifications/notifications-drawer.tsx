import * as React from "react";

import CloseIcon from "@mui/icons-material/Close";
import MarkEmailReadIcon from "@mui/icons-material/MarkEmailRead";
import {
  Box,
  Button,
  Divider,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Typography,
} from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";

import { NotificationListItem } from "../../models";
import {
  notificationDetailsQueryOptions,
  notificationsListQueryOptions,
  unreadCountQueryOptions,
} from "../../query-options";
import SideDrawerContent from "../side-drawer/side-drawer-content";
import useSideDrawer from "../side-drawer/use-side-drawer";

const NotificationsDrawer = () => {
  const { closeDrawer, openDrawer } = useSideDrawer();
  const { data } = useSuspenseQuery(notificationsListQueryOptions());
  // Support both paged shape { items, page, pageSize, total } and raw array []
  const raw: unknown = data as unknown;
  let items: NotificationListItem[];
  if (Array.isArray(raw)) {
    items = raw as NotificationListItem[];
  } else {
    const paged = raw as any;
    items = (paged.items ?? []) as NotificationListItem[];
  }
  const page =
    !Array.isArray(raw) && typeof (raw as any).page === "number"
      ? (raw as any).page
      : 0;
  const pageSize =
    !Array.isArray(raw) && typeof (raw as any).pageSize === "number"
      ? (raw as any).pageSize
      : items.length;
  const total =
    !Array.isArray(raw) && typeof (raw as any).total === "number"
      ? (raw as any).total
      : items.length;
  const queryClient = useQueryClient();

  const markRead = useMutation({
    mutationFn: async (id: string) => {
      await fetch(`/api/notifications/${id}/read`, { method: "POST" });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
      queryClient.invalidateQueries({
        queryKey: ["notifications-unread-count"],
      });
    },
  });

  const openDetails = (id: string) => {
    // Replace content in the drawer with details view
    openDrawer(
      "Notification",
      <NotificationDetailsView id={id} onClose={closeDrawer} />,
    );
  };

  return (
    <SideDrawerContent>
      <Box sx={{ p: 2, display: "flex", alignItems: "center" }}>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          Notifications
        </Typography>
        <IconButton aria-label="Close" onClick={closeDrawer}>
          <CloseIcon />
        </IconButton>
      </Box>
      <Divider />
      <List dense disablePadding>
        {items.map((n: NotificationListItem) => {
          const isUnread = n.isRead === false;
          const secondary = isUnread ? (
            <IconButton
              edge="end"
              aria-label="Mark as read"
              onClick={(e) => {
                e.stopPropagation();
                markRead.mutate(n.id);
              }}
            >
              <MarkEmailReadIcon />
            </IconButton>
          ) : undefined;
          const titleWeight = n.isRead ? 500 : 700;
          const titleNode = (
            <Typography variant="subtitle1" fontWeight={titleWeight}>
              {n.title}
            </Typography>
          );
          const bodyNode = (
            <Typography variant="body2" color="text.secondary">
              {n.body}
            </Typography>
          );
          return (
            <React.Fragment key={n.id}>
              <ListItem
                onClick={() => openDetails(n.id)}
                sx={{ alignItems: "flex-start", py: 1, cursor: "pointer" }}
                secondaryAction={secondary}
              >
                <ListItemText primary={titleNode} secondary={bodyNode} />
              </ListItem>
              <Divider component="li" />
            </React.Fragment>
          );
        })}
      </List>
      {page * pageSize < total && (
        <Box sx={{ p: 2, display: "flex", justifyContent: "center" }}>
          <Button
            variant="outlined"
            onClick={() => {
              // naive pagination: refetch with next page
              const next = page + 1;
              queryClient.invalidateQueries({ queryKey: ["notifications"] });
            }}
          >
            Load more
          </Button>
        </Box>
      )}
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
  const { data } = useSuspenseQuery(notificationDetailsQueryOptions(id));
  const queryClient = useQueryClient();

  const markRead = useMutation({
    mutationFn: async () => {
      await fetch(`/api/notifications/${id}/read`, { method: "POST" });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
      queryClient.invalidateQueries({
        queryKey: ["notifications-unread-count"],
      });
    },
  });

  const details = data;
  return (
    <SideDrawerContent>
      <Box sx={{ p: 2, display: "flex", alignItems: "center" }}>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          {details.title}
        </Typography>
        <IconButton aria-label="Close" onClick={onClose}>
          <CloseIcon />
        </IconButton>
      </Box>
      <Divider />
      <Box sx={{ p: 2 }}>
        <Typography variant="body1" sx={{ mb: 1 }}>
          {details.body}
        </Typography>
        {(() => {
          const attempts = ((details as any).deliveryAttempts ?? []) as any[];
          if (attempts.length === 0) return null;
          return (
            <Box sx={{ mt: 2 }}>
              <Typography variant="subtitle1">Delivery Attempts</Typography>
              {attempts.map((a) => {
                const failureSuffix = a.failureReason
                  ? ` (${a.failureReason})`
                  : "";
                return (
                  <Typography
                    key={`${a.channel}-${a.attemptNumber}`}
                    variant="body2"
                    color="text.secondary"
                  >
                    {a.attemptedOn}: {a.channel} → {a.status}
                    {failureSuffix}
                  </Typography>
                );
              })}
            </Box>
          );
        })()}
        <Box sx={{ mt: 2 }}>
          <Button
            variant="contained"
            startIcon={<MarkEmailReadIcon />}
            onClick={() => markRead.mutate()}
            disabled={details.isRead}
          >
            Mark as read
          </Button>
        </Box>
      </Box>
    </SideDrawerContent>
  );
};

export default NotificationsDrawer;
