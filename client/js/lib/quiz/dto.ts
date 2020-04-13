import { QuizInfo, Quiz } from "./models";
import { Task } from "../task/models";

export class CreateQuizRequest {
    public tasks: string[];
}

export class GetAllQuizzesResponse {
    public quizzes: QuizInfoDto[];
}

export class QuizInfoDto {
    public quizId: string;

    public static toInfo(quizInfo: QuizInfoDto): QuizInfo {
        return {
            quizId: quizInfo.quizId,
        };
    }
}

export class TaskResponse {
    public taskId: string;
    public question: string;
    public answer: string;
    public variants: string[];
    public static toModel(task: TaskResponse): Task {
        return {
            asnwer: task.answer,
            question: task.question,
            taskId: task.taskId,
            variants: task.variants,
        };
    }
}

export class GetQuizResponse {
    public quizId: string;
    public tasks: TaskResponse[];
    public static toModel(quiz: GetQuizResponse): Quiz {
        return {
            quizId: quiz.quizId,
            tasks: quiz.tasks.map((t) => TaskResponse.toModel(t)),
        };
    }
}
