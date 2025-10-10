import * as React from "react";

import {
  CalendarToday,
  CheckCircle,
  Delete,
  Edit,
  Numbers,
  Receipt,
  TextFields,
  Visibility,
} from "@mui/icons-material";
import {
  alpha,
  Box,
  Card,
  CardActions,
  CardContent,
  Divider,
  Grid,
  IconButton,
  Tooltip,
  Typography,
  useTheme,
} from "@mui/material";

import {
  ColumnType,
  getStatusFromName,
  ListItem,
  ListItemDefinition,
} from "../models";
import StatusChip from "./status-chip";

interface Props {
  item: ListItem;
  definition: ListItemDefinition;
  onViewItem?: (listId: string, itemId: number) => void;
  onEditItem?: (listId: string, itemId: number) => void;
  onDeleteItem?: (listId: string, itemId: number) => void;
}

const ItemCard = ({
  item,
  definition,
  onViewItem,
  onEditItem,
  onDeleteItem,
}: Props) => {
  const theme = useTheme();
  const rawStatus = item.bag.status;
  const status = getStatusFromName(definition.statuses, rawStatus);
  const statusChip = status && <StatusChip status={status} />;

  const viewAction = onViewItem ? (
    <Tooltip title={`View item ${item.id}`}>
      <IconButton
        onClick={() => onViewItem(definition.id!, item.id!)}
        sx={{
          color: "primary.main",
          "&:hover": {
            backgroundColor: alpha(theme.palette.primary.main, 0.1),
          },
        }}
      >
        <Visibility />
      </IconButton>
    </Tooltip>
  ) : null;

  const editAction = onEditItem ? (
    <Tooltip title={`Edit item ${item.id}`}>
      <IconButton
        onClick={() => onEditItem(definition.id!, item.id!)}
        sx={{
          color: "secondary.main",
          "&:hover": {
            backgroundColor: alpha(theme.palette.secondary.main, 0.1),
          },
        }}
      >
        <Edit />
      </IconButton>
    </Tooltip>
  ) : null;

  const deleteAction = onDeleteItem ? (
    <Tooltip title={`Delete item ${item.id}`}>
      <IconButton
        onClick={() => onDeleteItem(definition.id!, item.id!)}
        sx={{
          color: "error.main",
          "&:hover": {
            backgroundColor: alpha(theme.palette.error.main, 0.1),
          },
        }}
      >
        <Delete />
      </IconButton>
    </Tooltip>
  ) : null;

  const getFieldIcon = (type: ColumnType) => {
    switch (type) {
      case ColumnType.Date:
        return <CalendarToday sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Boolean:
        return <CheckCircle sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Number:
        return <Numbers sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Text:
      default:
        return <TextFields sx={{ fontSize: 16, color: "text.secondary" }} />;
    }
  };

  const getDisplayValue = (
    rawValue: any,
    type: ColumnType,
  ): React.ReactNode => {
    if (rawValue === null || rawValue === undefined || rawValue === "") {
      return "-";
    }

    switch (type) {
      case ColumnType.Date:
        return new Date(rawValue).toLocaleDateString();
      case ColumnType.Boolean:
        return rawValue ? "True" : "False";
      case ColumnType.Number:
      case ColumnType.Text:
      default:
        return String(rawValue);
    }
  };

  return (
    <Card
      variant="outlined"
      sx={{
        transition: "all 0.2s ease-in-out",
        "&:hover": {
          transform: "translateY(-2px)",
          boxShadow: theme.shadows[4],
          borderColor: alpha(theme.palette.primary.main, 0.5),
          "& .card-actions": {
            opacity: 1,
          },
        },
      }}
    >
      {/* Header with ID and status */}
      <Box
        sx={{
          p: 3,
          pb: 2,
          backgroundColor: alpha(theme.palette.primary.main, 0.03),
          borderRadius: `${theme.shape.borderRadius}px ${theme.shape.borderRadius}px 0 0`,
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
          <Receipt
            sx={{
              fontSize: 20,
              color: "primary.main",
              p: 0.5,
              backgroundColor: alpha(theme.palette.primary.main, 0.1),
              borderRadius: 1,
            }}
          />
          {statusChip && <Box sx={{ ml: "auto" }}>{statusChip}</Box>}
        </Box>

        <Typography
          variant="h6"
          component="div"
          fontWeight="600"
          sx={{
            fontSize: "1.25rem",
            lineHeight: 1.3,
            color: "text.primary",
          }}
        >
          ID {item.id}
        </Typography>
      </Box>

      <CardContent sx={{ p: 3, pt: 3 }}>
        {/* Fields Grid */}
        <Grid container spacing={2}>
          {definition.columns.map((column, index) => {
            const key = column.property ?? column.name;
            const rawValue = item.bag[key];
            const displayValue = getDisplayValue(rawValue, column.type);
            const isLastItem = index === definition.columns.length - 1;
            const fontWeight =
              column.type === ColumnType.Number ? "medium" : "normal";

            return (
              <React.Fragment key={key}>
                <Grid size={{ xs: 5 }}>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      height: "100%",
                      minHeight: 40,
                    }}
                  >
                    {getFieldIcon(column.type)}
                    <Typography
                      variant="caption"
                      color="text.secondary"
                      fontWeight="medium"
                      sx={{ textTransform: "uppercase", letterSpacing: 0.5 }}
                    >
                      {column.name}
                    </Typography>
                  </Box>
                </Grid>
                <Grid size={{ xs: 7 }}>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      height: "100%",
                      minHeight: 40,
                      pl: 2,
                    }}
                  >
                    <Typography
                      variant="body1"
                      color="text.primary"
                      fontWeight={fontWeight}
                      sx={{
                        fontSize: "1rem",
                        lineHeight: 1.5,
                      }}
                    >
                      {displayValue}
                    </Typography>
                  </Box>
                </Grid>
                {!isLastItem && (
                  <Grid size={{ xs: 12 }} sx={{ my: 1.5 }}>
                    <Divider
                      sx={{
                        mx: -3,
                        borderColor: alpha(theme.palette.divider, 0.6),
                      }}
                    />
                  </Grid>
                )}
              </React.Fragment>
            );
          })}
        </Grid>
      </CardContent>

      <Divider sx={{ borderColor: alpha(theme.palette.divider, 0.6) }} />

      {/* Action buttons */}
      <CardActions
        className="card-actions"
        sx={{
          justifyContent: "flex-end",
          p: 2,
          opacity: 0.7,
          transition: "opacity 0.2s ease-in-out",
          gap: 0.5,
        }}
      >
        {viewAction}
        {editAction}
        {deleteAction}
      </CardActions>
    </Card>
  );
};

export default ItemCard;
