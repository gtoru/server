import { Task } from "../task/models";

export type QuizId = string;

export type QuizName = string;

export class QuizInfo {
    public quizName: QuizName;
    public quizId: QuizId;
}

export class Quiz {
    public quizName: QuizName;
    public quizId: QuizId;
    public tasks: Task[];
}
