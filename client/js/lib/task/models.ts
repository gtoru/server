export type Question = string;

export type Answer = string;

export type TaskId = string;

export class Task {
    public taskId: TaskId;
    public question: Question;
    public answer: Answer;
    public variants: Answer[];
}
