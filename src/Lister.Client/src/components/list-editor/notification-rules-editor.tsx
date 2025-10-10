import * as React from "react";

import {
  Add,
  Delete,
  MailOutline,
  Notifications as NotificationsIcon,
} from "@mui/icons-material";
import {
  Box,
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormGroup,
  FormHelperText,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

import {
  NotificationChannel,
  NotificationChannelType,
  NotificationSchedule,
  NotificationScheduleType,
  NotificationTrigger,
  NotificationTriggerType,
  Status,
} from "../../models";

import type { NotificationRuleFormValue } from "./types";

interface Props {
  rules: NotificationRuleFormValue[];
  statuses: Status[];
  onAddRule: () => void;
  onUpdateRule: (
    clientId: string,
    updater: (rule: NotificationRuleFormValue) => NotificationRuleFormValue,
  ) => void;
  onRemoveRule: (rule: NotificationRuleFormValue) => void;
}

const triggerOptions: { label: string; value: NotificationTriggerType }[] = [
  { label: "Item created", value: "ItemCreated" },
  { label: "Item updated", value: "ItemUpdated" },
  { label: "Item deleted", value: "ItemDeleted" },
  { label: "Status changed", value: "StatusChanged" },
];

const channelOptions: {
  label: string;
  value: NotificationChannelType;
  icon: React.ReactNode;
  requiresAddress: boolean;
}[] = [
  {
    label: "In-app",
    value: "InApp",
    icon: <NotificationsIcon fontSize="small" />,
    requiresAddress: false,
  },
  {
    label: "Email",
    value: "Email",
    icon: <MailOutline fontSize="small" />,
    requiresAddress: true,
  },
];

const scheduleOptions: { label: string; value: NotificationScheduleType }[] = [
  { label: "Immediately", value: "Immediate" },
];

const ensureSettings = (channel: NotificationChannel): NotificationChannel => ({
  ...channel,
  settings: channel.settings ?? {},
});

const NotificationRulesEditor = ({
  rules,
  statuses,
  onAddRule,
  onRemoveRule,
  onUpdateRule,
}: Props) => {
  const statusNames = React.useMemo(
    () => statuses.map((status) => status.name),
    [statuses],
  );
  const handleTriggerTypeChange = (
    rule: NotificationRuleFormValue,
    nextType: NotificationTriggerType,
  ) => {
    const nextTrigger: NotificationTrigger = {
      type: nextType,
    };

    if (nextType === "StatusChanged") {
      nextTrigger.fromValue = null;
      nextTrigger.toValue = null;
    }

    onUpdateRule(rule.clientId, (prev) => ({
      ...prev,
      trigger: nextTrigger,
    }));
  };

  const handleTriggerStatusChange = (
    rule: NotificationRuleFormValue,
    field: "fromValue" | "toValue",
    value: string | null,
  ) => {
    onUpdateRule(rule.clientId, (prev) => ({
      ...prev,
      trigger: {
        ...prev.trigger,
        [field]: value,
      },
    }));
  };

  const handleChannelToggle = (
    rule: NotificationRuleFormValue,
    channelType: NotificationChannelType,
    checked: boolean,
  ) => {
    onUpdateRule(rule.clientId, (prev) => {
      const existing = prev.channels;
      if (checked) {
        if (existing.some((channel) => channel.type === channelType)) {
          return prev;
        }
        const nextChannel: NotificationChannel = ensureSettings({
          type: channelType,
          address: undefined,
        });
        if (channelType === "Email") {
          nextChannel.address = "";
        }
        return {
          ...prev,
          channels: [...existing, nextChannel],
        };
      }

      return {
        ...prev,
        channels: existing.filter((channel) => channel.type !== channelType),
      };
    });
  };

  const handleChannelAddressChange = (
    rule: NotificationRuleFormValue,
    channelType: NotificationChannelType,
    address: string,
  ) => {
    onUpdateRule(rule.clientId, (prev) => ({
      ...prev,
      channels: prev.channels.map((channel) =>
        channel.type === channelType ? { ...channel, address } : channel,
      ),
    }));
  };

  const handleScheduleChange = (
    rule: NotificationRuleFormValue,
    scheduleType: NotificationScheduleType,
  ) => {
    const nextSchedule: NotificationSchedule = {
      type: scheduleType,
    };
    onUpdateRule(rule.clientId, (prev) => ({
      ...prev,
      schedule: nextSchedule,
    }));
  };

  const handleTemplateChange = (
    rule: NotificationRuleFormValue,
    templateId: string,
  ) => {
    onUpdateRule(rule.clientId, (prev) => ({
      ...prev,
      templateId: templateId || undefined,
    }));
  };

  const rulesView =
    rules.length === 0 ? (
      <Typography color="text.secondary">
        No notification rules configured yet.
      </Typography>
    ) : (
      <Stack spacing={2}>
        {rules.map((rule, index) => {
          const channels = rule.channels;
          const hasChannel = (type: NotificationChannelType) =>
            channels.some((channel) => channel.type === type);

          const emailChannel = channels.find(
            (channel) => channel.type === "Email",
          );

          const emailField = emailChannel ? (
            <TextField
              label="Email address"
              value={emailChannel.address ?? ""}
              onChange={(event) =>
                handleChannelAddressChange(rule, "Email", event.target.value)
              }
              placeholder="name@example.com"
            />
          ) : null;

          return (
            <Paper key={rule.clientId} variant="outlined" sx={{ p: 2 }}>
              <Stack spacing={2}>
                <Stack
                  direction="row"
                  alignItems="center"
                  justifyContent="space-between"
                >
                  <Typography fontWeight={600}>Rule {index + 1}</Typography>
                  <Button
                    color="error"
                    startIcon={<Delete />}
                    onClick={() => onRemoveRule(rule)}
                    size="small"
                  >
                    Remove
                  </Button>
                </Stack>

                <FormControl fullWidth>
                  <InputLabel id={`trigger-${rule.clientId}`}>
                    Trigger
                  </InputLabel>
                  <Select
                    labelId={`trigger-${rule.clientId}`}
                    label="Trigger"
                    value={rule.trigger.type}
                    onChange={(event) =>
                      handleTriggerTypeChange(
                        rule,
                        event.target.value as NotificationTriggerType,
                      )
                    }
                  >
                    {triggerOptions.map((option) => (
                      <MenuItem key={option.value} value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>

                {rule.trigger.type === "StatusChanged" && (
                  <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                    <FormControl fullWidth>
                      <InputLabel id={`from-${rule.clientId}`}>
                        From status
                      </InputLabel>
                      <Select
                        labelId={`from-${rule.clientId}`}
                        label="From status"
                        value={rule.trigger.fromValue ?? ""}
                        onChange={(event) =>
                          handleTriggerStatusChange(
                            rule,
                            "fromValue",
                            event.target.value === ""
                              ? null
                              : (event.target.value as string),
                          )
                        }
                      >
                        <MenuItem value="">
                          <em>Any</em>
                        </MenuItem>
                        {statusNames.map((name) => (
                          <MenuItem key={name} value={name}>
                            {name}
                          </MenuItem>
                        ))}
                      </Select>
                      <FormHelperText>
                        Leave as Any to match all starting statuses.
                      </FormHelperText>
                    </FormControl>

                    <FormControl fullWidth>
                      <InputLabel id={`to-${rule.clientId}`}>
                        To status
                      </InputLabel>
                      <Select
                        labelId={`to-${rule.clientId}`}
                        label="To status"
                        value={rule.trigger.toValue ?? ""}
                        onChange={(event) =>
                          handleTriggerStatusChange(
                            rule,
                            "toValue",
                            event.target.value === ""
                              ? null
                              : (event.target.value as string),
                          )
                        }
                      >
                        <MenuItem value="">
                          <em>Any</em>
                        </MenuItem>
                        {statusNames.map((name) => (
                          <MenuItem key={name} value={name}>
                            {name}
                          </MenuItem>
                        ))}
                      </Select>
                      <FormHelperText>
                        Choose a destination status to watch for, or Any.
                      </FormHelperText>
                    </FormControl>
                  </Stack>
                )}

                <Box>
                  <Typography fontWeight={600} gutterBottom>
                    Channels
                  </Typography>
                  <FormGroup row>
                    {channelOptions.map((option) => (
                      <FormControlLabel
                        key={option.value}
                        control={
                          <Checkbox
                            checked={hasChannel(option.value)}
                            onChange={(event) =>
                              handleChannelToggle(
                                rule,
                                option.value,
                                event.target.checked,
                              )
                            }
                          />
                        }
                        label={
                          <Stack
                            direction="row"
                            spacing={1}
                            alignItems="center"
                          >
                            {option.icon}
                            <span>{option.label}</span>
                          </Stack>
                        }
                      />
                    ))}
                  </FormGroup>
                  {emailField}
                </Box>

                <FormControl fullWidth>
                  <InputLabel id={`schedule-${rule.clientId}`}>
                    Schedule
                  </InputLabel>
                  <Select
                    labelId={`schedule-${rule.clientId}`}
                    label="Schedule"
                    value={rule.schedule.type}
                    onChange={(event) =>
                      handleScheduleChange(
                        rule,
                        event.target.value as NotificationScheduleType,
                      )
                    }
                  >
                    {scheduleOptions.map((option) => (
                      <MenuItem key={option.value} value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                  <FormHelperText>
                    Notifications are sent immediately by default.
                  </FormHelperText>
                </FormControl>

                <TextField
                  label="Template ID (optional)"
                  value={rule.templateId ?? ""}
                  onChange={(event) =>
                    handleTemplateChange(rule, event.target.value)
                  }
                  placeholder="Enter a template identifier"
                />
              </Stack>
            </Paper>
          );
        })}
      </Stack>
    );

  return (
    <Stack spacing={2}>
      <Box>
        <Button
          onClick={onAddRule}
          startIcon={<Add />}
          variant="outlined"
          size="small"
        >
          Add rule
        </Button>
      </Box>

      {rulesView}
    </Stack>
  );
};

export default NotificationRulesEditor;
