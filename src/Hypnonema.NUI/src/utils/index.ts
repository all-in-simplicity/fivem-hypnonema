import Schedule, { Recurrence } from "../types/schedule";
import { ByWeekday, Frequency, RRule } from "rrule";

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
