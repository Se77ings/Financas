/**
 * Interface para classes que suportam desserialização
 */
export interface Deserializable<T> {
  fromJSON(data: any): T;
}

/**
 * Utilitário genérico para desserializar dados JSON em instâncias de classes
 */
export class Deserializer {
  /**
   * Desserializa um objeto JSON para uma instância de classe
   * @param data Dados JSON
   * @param ctor Construtor da classe alvo (deve ter método estático fromJSON)
   * @returns Instância desserializada
   */
  static deserialize<T>(
    data: any,
    ctor: { fromJSON: (data: any) => T }
  ): T {
    return ctor.fromJSON(data);
  }

  /**
   * Desserializa um array de objetos JSON
   * @param dataArray Array de dados JSON
   * @param ctor Construtor da classe alvo (deve ter método estático fromJSON)
   * @returns Array de instâncias desserializadas
   */
  static deserializeArray<T>(
    dataArray: any[],
    ctor: { fromJSON: (data: any) => T }
  ): T[] {
    return (dataArray || []).map((item) => this.deserialize(item, ctor));
  }
}
