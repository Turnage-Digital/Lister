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
  Divider,
  Grid,
  IconButton,
  Stack,
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
        color="primary"
      >
        <Visibility />
      </IconButton>
    </Tooltip>
  ) : null;

  const editAction = onEditItem ? (
    <Tooltip title={`Edit item ${item.id}`}>
      <IconButton
        onClick={() => onEditItem(definition.id!, item.id!)}
        color="primary"
      >
        <Edit />
      </IconButton>
    </Tooltip>
  ) : null;

  const deleteAction = onDeleteItem ? (
    <Tooltip title={`Delete item ${item.id}`}>
      <IconButton
        onClick={() => onDeleteItem(definition.id!, item.id!)}
        color="error"
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

  const statusChipNode = statusChip ? (
    <Box sx={{ ml: "auto" }}>{statusChip}</Box>
  ) : null;

  return (
    <Card
      variant="outlined"
      sx={{
        display: "flex",
        flexDirection: "column",
        gap: 3,
        p: 3,
        maxWidth: 500,
        borderColor: alpha(theme.palette.primary.main, 0.12),
        transition: "border-color 0.2s ease, box-shadow 0.2s ease",
        "&:hover": {
          borderColor: alpha(theme.palette.primary.main, 0.3),
          boxShadow: theme.shadows[3],
        },
      }}
    >
      <Stack
        direction="row"
        alignItems="center"
        justifyContent="space-between"
        spacing={2}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            width: 44,
            height: 44,
            borderRadius: 2,
            backgroundColor: alpha(theme.palette.primary.main, 0.1),
            color: "primary.main",
          }}
        >
          <Receipt sx={{ fontSize: 22 }} />
        </Box>
        {statusChipNode}
      </Stack>

      <Typography variant="h6" component="div">
        ID {item.id}
      </Typography>

      <Box sx={{ flexGrow: 1 }}>
        <Grid container spacing={{ xs: 3, md: 3.5 }}>
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
      </Box>

      <CardActions
        className="card-actions"
        sx={{ justifyContent: "flex-end", gap: 1, pt: 1 }}
      >
        {viewAction}
        {editAction}
        {deleteAction}
      </CardActions>
    </Card>
  );
};

export default ItemCard;
