import Schedule, { Recurrence } from "../types/schedule";
import { ByWeekday, Frequency, RRule } from "rrule";
import { formatDistance } from "date-fns";

export const getScheduleRuleText = (schedule: Schedule): string => {
  const rule = new RRule({
    freq: schedule.rule as number as Frequency,
    interval: schedule.interval,
    dtstart: new Date(schedule.startDateTime),
    until: schedule.endDate ? new Date(schedule.endDate!) : null,
    byweekday:
      schedule.rule !== Recurrence.Daily
        ? (schedule.dayOfWeek as ByWeekday)
        : null,
  });
  return rule.toText();
};

export const getNextScheduleOccurrence = (schedule: Schedule): string => {
  const rule = new RRule({
    freq: schedule.rule as number as Frequency,
    interval: schedule.interval,
    count: 1,
    dtstart: new Date(schedule.startDateTime),
    until: schedule.endDate ? new Date(schedule.endDate!) : null,
    byweekday:
      schedule.rule !== Recurrence.Daily
        ? (schedule.dayOfWeek as ByWeekday)
        : null,
  });
  const occurrences = rule.all();
  return formatDistance(occurrences[0] || "", new Date());
};
